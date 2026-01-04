using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RefConnect.Services.Interfaces;

namespace RefConnect.Services.Implementations;

public class RefinePostTextAIService : IRefinePostTextAI
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiUrl;
    private readonly ILogger<RefinePostTextAIService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public RefinePostTextAIService(HttpClient httpClient, IConfiguration configuration, ILogger<RefinePostTextAIService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("RefinePostTextAI:ApiKey not configured");
        _apiUrl = configuration["OpenAI:ApiUrl"] ?? throw new ArgumentNullException("RefinePostTextAI:ApiUrl not configured");
        _logger = logger;

        if (Uri.TryCreate(_apiUrl, UriKind.Absolute, out var baseUri))
        {
            _httpClient.BaseAddress = baseUri;
        }
    }

    public async Task<string> RefineTextAsync(string inputText, CancellationToken ct = default)
    {
        // preserve your original prompt but parameterize the inputText into it
        var systemPrompt = @"Vei primi un text in care se vor discuta faze si idei despre arbitraj de fotbal, vreau atunci cand se
            poate sa inlocuiesti termeni populari de tipul 'penalty' , 'careu' cu termeni specifici din
            cartea Legile Jocului (Laws of the Game, Romanian) , precum 'lovitura de pedeapsa' sau 'suprafata de pedeapsa/poarta' 
            ca si cum ai fi un specialist ROMAN in Legile Jocului , vreau sa folosesti cat mai mult termenii din carte. 
            Reguli:
            1. Trebuie ca modificarile pe care le faci sa pastreze sensul textului dat de input.
            2. Foloseste cat mai mult termenii din cartea Legile Jocului.
            3. Textul trebuie sa fie coerent si usor de citit.

            Vei intoarce un obiect JSON cu o singura proprietate 'refined_text' care va contine textul rafinat.
            Exemplu de raspuns:
            {""refined_text"": ""Textul rafinat aici...""}

            Exemplu practic:
            Input: 'Arbitrul a acordat un penalty dupa ce un jucator a faultat in careu.'
            Output: { 'refined_text': 'Arbitrul a acordat o lovitura de pedeapsa dupa ce un jucator a comis un fault in suprafata de pedeapsa.' }";

        var userPrompt = $"Text de rafinat: \"{inputText}\"";

        var requestBody = new
        {
            //groq
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.7,
            max_tokens = 300
        };

        var requestJson = JsonSerializer.Serialize(requestBody, _jsonOptions);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

     
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        try
        {
            _logger.LogInformation("Sending refine request to OpenAI API");

            var postUrl = _httpClient.BaseAddress == null ? _apiUrl : "chat/completions";
            var response = await _httpClient.PostAsync(postUrl, content, ct);
            var responseString = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API error: {StatusCode} - {Content}", response.StatusCode, responseString);
                return inputText;
            }

        
            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString, _jsonOptions);
            var assistantMessage = openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(assistantMessage))
            {
                _logger.LogWarning("OpenAI returned empty assistant message");
                return inputText;
            }

            _logger.LogInformation("OpenAI assistant message: {Msg}", assistantMessage);

            RefineResponse? data = null;
            try
            {
                data = JsonSerializer.Deserialize<RefineResponse>(assistantMessage, _jsonOptions);
            }
            catch (JsonException)
            {
               
                var start = assistantMessage.IndexOf('{');
                var end = assistantMessage.LastIndexOf('}');
                if (start >= 0 && end > start)
                {
                    var jsonPart = assistantMessage.Substring(start, end - start + 1);
                    try
                    {
                        data = JsonSerializer.Deserialize<RefineResponse>(jsonPart, _jsonOptions);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse refined JSON from assistant message");
                    }
                }
            }

            if (data == null || string.IsNullOrWhiteSpace(data.RefinedText))
            {
                _logger.LogWarning("Refined text missing in assistant response; returning original input");
                return inputText;
            }

            return data.RefinedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling OpenAI for refine text");
            return inputText;
        }
    }  
    public async Task<bool> IsContentAppropriateAsync(string content, CancellationToken ct = default)
    {
        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = "You are a content moderator. Determine if the given content is appropriate for all audiences. The content is in Romanian." },
                new { role = "user", content = $"Is the following content appropriate? \"{content}\" Respond with 'yes' or 'no'." }
            },
            temperature = 0.0,
            max_tokens = 10
        };

        var requestJson = JsonSerializer.Serialize(requestBody, _jsonOptions);
        using var contentHttp = new StringContent(requestJson, Encoding.UTF8, "application/json");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        try
        {
            _logger.LogInformation("Sending content appropriateness request to OpenAI API");

            var postUrl = _httpClient.BaseAddress == null ? _apiUrl : "chat/completions";
            var response = await _httpClient.PostAsync(postUrl, contentHttp, ct);
            var responseString = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API error: {StatusCode} - {Content}", response.StatusCode, responseString);
                return true; 
            }

            var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString, _jsonOptions);
            var assistantMessage = openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(assistantMessage))
            {
                _logger.LogWarning("OpenAI returned empty assistant message");
                return true; 
            }

            _logger.LogInformation("OpenAI assistant message: {Msg}", assistantMessage);

            return assistantMessage.Trim().ToLowerInvariant().StartsWith("yes");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling OpenAI for content appropriateness");
            return true; 
        }
    }

    private class OpenAiResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    private class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    private class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class RefineResponse
    {
        [JsonPropertyName("refined_text")]
        public string? RefinedText { get; set; }
    }
}
