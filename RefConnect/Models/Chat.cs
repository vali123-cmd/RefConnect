using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefConnect.Models;

public class Chat
{
    [Key]
    public string ChatId { get; set; }
    public string ChatType { get; set; } // direct / group
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    [Required(ErrorMessage = "MatchId is required for match chats")]
    public string? MatchId { get; set; }
    [ForeignKey("MatchId")]

    [Required(ErrorMessage = "Match is required for match chats")]
    public Match Match { get; set; } = new Match();

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
}