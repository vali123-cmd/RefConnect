using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefConnect.Models;

public class Chat
{
    [Key]
    public int ChatId { get; set; }
    public string ChatType { get; set; } // direct / group
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    public int? MatchId { get; set; }
    [ForeignKey("MatchId")]
    public Match Match { get; set; }

    public virtual ICollection<ChatUser> ChatUsers { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    
}