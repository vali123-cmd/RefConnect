using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Posts;

public class CreatePostDto
{
   
    public string? MediaType { get; set; }

    public string? MediaUrl { get; set; }

    [StringLength(2000, ErrorMessage = "Descrierea poate avea maximum 2000 de caractere.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "UserId este obligatoriu.")]
    public string UserId { get; set; }
}

