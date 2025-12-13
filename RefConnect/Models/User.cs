namespace RefConnect.Models;

public class User
{
    public int UserId { get; set; }
    public string Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<MatchAssignment> MatchAssignments { get; set; }
    public ICollection<Post> Posts { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<ChatUser> ChatUsers { get; set; }
}