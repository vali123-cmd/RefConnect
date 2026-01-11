using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Comments;

public class CreateCommentDto
{
    [Required(ErrorMessage = "Conținutul comentariului este obligatoriu.")]
    [StringLength(2000, ErrorMessage = "Comentariul poate avea maximum 2000 de caractere.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Id-ul postării este obligatoriu.")]
    public string PostId { get; set; }

    [Required(ErrorMessage = "Id-ul utilizatorului este obligatoriu.")]
    public string UserId { get; set; }

    public string? ParentCommentId { get; set; }
}

