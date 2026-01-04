using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Post
{
    [Key]
    public string PostId { get; set; } = null!;

   
    public string MediaType { get; set; } = null!; 

    public string MediaUrl { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    public string UserId { get; set; } = null!;

    [Required]
    public int LikeCount { get; set; } = 0;
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}