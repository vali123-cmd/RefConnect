using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.ChatUsers;

public class UpdateChatUserDto
{
    [Required]
    public string ChatId { get; set; }

    [Required]
    public string UserId { get; set; }
}

