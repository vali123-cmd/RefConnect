using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.ChatUsers;

public class CreateChatUserDto
{
    [Required]
    public string ChatId { get; set; }

    [Required]
    public string UserId { get; set; }
}

