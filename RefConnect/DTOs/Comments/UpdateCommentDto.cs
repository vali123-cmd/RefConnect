using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Comments;

public class UpdateCommentDto
{
    [Required]
    public string Content { get; set; }
}

