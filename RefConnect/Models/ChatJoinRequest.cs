namespace RefConnect.Models;








using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ChatJoinRequest
{
    [Key]
    public string ChatJoinRequestId { get; set; }

    [Required]
    public string ChatId { get; set; }
    [ForeignKey("ChatId")]
    public virtual Chat Chat { get; set; }

    [Required]
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    public DateTime RequestedAt { get; set; }

    [Required]
    public string Status { get; set; } 
}