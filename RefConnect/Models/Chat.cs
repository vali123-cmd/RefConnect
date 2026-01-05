using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefConnect.Models;

public class Chat
{
    [Key]
    public string ChatId { get; set; }

    [Required]
    public string ChatType { get; set; } // direct / group
    public DateTime CreatedAt { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string CreatedByUserId { get; set; }
    [ForeignKey("CreatedByUserId")]

    public virtual ApplicationUser CreatedByUser { get; set; }
    
    
    public string? MatchId { get; set; }
    [ForeignKey("MatchId")]
    public virtual Match? Match { get; set; }

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
}