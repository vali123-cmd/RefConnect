using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Message
{
    [Key]
    public string MessageId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    public string ChatId { get; set; }
    public virtual Chat Chat { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}