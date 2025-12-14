namespace RefConnect.DTOs.Messages;

public class MessageDto
{
    public string MessageId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public string ChatId { get; set; }
    public string UserId { get; set; }
}


