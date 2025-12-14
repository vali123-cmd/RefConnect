using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Messages;

public class UpdateMessageDto
{
    [Required]
    public string Content { get; set; }
}

