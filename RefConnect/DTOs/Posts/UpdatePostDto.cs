using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Posts;

public class UpdatePostDto
{

    public string MediaType { get; set; }

    public string MediaUrl { get; set; }

    public string Description { get; set; }
}

