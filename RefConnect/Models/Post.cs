using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Post
{
    [Key]
    public int PostId { get; set; }
    public string MediaType { get; set; } // video / image
    public string MediaUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }
}