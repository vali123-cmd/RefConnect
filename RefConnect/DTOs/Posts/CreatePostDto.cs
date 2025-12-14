using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Posts;

public class CreatePostDto
{
    [Required]
    public string MediaType { get; set; }

    [Required]
    public string MediaUrl { get; set; }

    public string Description { get; set; }

    [Required]
    public string UserId { get; set; }
}

