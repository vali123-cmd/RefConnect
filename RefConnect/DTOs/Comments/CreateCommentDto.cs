using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Comments;

public class CreateCommentDto
{
    [Required]
    public string Content { get; set; }

    [Required]
    public string PostId { get; set; }

    [Required]
    public string UserId { get; set; }

    public string? ParentCommentId { get; set; }
}

