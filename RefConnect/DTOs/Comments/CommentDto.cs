namespace RefConnect.DTOs.Comments;

public class CommentDto
{
    public string CommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PostId { get; set; }
    public string UserId { get; set; }
    public string? ParentCommentId { get; set; }
}


