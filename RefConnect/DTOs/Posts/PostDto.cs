

using RefConnect.DTOs.Comments;

namespace RefConnect.DTOs.Posts;


public class PostDto
{
    public string PostId { get; set; }
    public string MediaType { get; set; }
    public string MediaUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<CommentDto> Comments { get; set; }

    public int LikeCount { get; set; }
    public string UserId { get; set; }

  


}


  public class CommentDto
{
    public string CommentId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PostId { get; set; }
    public string UserId { get; set; }
    public string? ParentCommentId { get; set; }
}