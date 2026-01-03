using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Message
{
    [Key]

    [Required]
    public string MessageId { get; set; } = null!;

    [Required]

    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    [Required]
    public string ChatId { get; set; } = null!;
 
    public virtual Chat Chat { get; set; } = null!;
    [Required]
    public string UserId { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}