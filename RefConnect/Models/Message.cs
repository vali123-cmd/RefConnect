using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Message
{
    [Key]
    public int MessageId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    public int ChatId { get; set; }
    public virtual Chat Chat { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
}