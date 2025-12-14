using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Messages;

public class CreateMessageDto
{
    [Required]
    public string Content { get; set; }

    [Required]
    public string ChatId { get; set; }

    [Required]
    public string UserId { get; set; }
}

