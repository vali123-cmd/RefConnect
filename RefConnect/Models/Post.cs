namespace RefConnect.Models;

public class Post
{
    public int PostId { get; set; }
    public string MediaType { get; set; } // video / image
    public string MediaUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<Comment> Comments { get; set; }
}