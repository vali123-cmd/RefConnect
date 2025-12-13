using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    public string Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MatchAssignment> MatchAssignments { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<ChatUser> ChatUsers { get; set; }
}