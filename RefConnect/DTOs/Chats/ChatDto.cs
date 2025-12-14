namespace RefConnect.DTOs.Chats;

public class ChatDto
{
    public string ChatId { get; set; }
    public string ChatType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string? MatchId { get; set; }
}


