using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Comment
{
    [Key]
    public int CommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public int PostId { get; set; }
    public virtual Post Post { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    public int? ParentCommentId { get; set; }
    public virtual Comment ParentComment { get; set; }

    public virtual ICollection<Comment> Replies { get; set; }
}