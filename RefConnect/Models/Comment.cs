using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Comment
{
    [Key]
    public string CommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public string PostId { get; set; }
    public virtual Post Post { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public string? ParentCommentId { get; set; }
    public virtual Comment ParentComment { get; set; }

    public virtual ICollection<Comment> Replies { get; set; }
}