using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.ChatUsers;

public class CreateChatUserDto
{
    [Required(ErrorMessage = "Id-ul chat-ului este obligatoriu.")]
    public string ChatId { get; set; }

    [Required(ErrorMessage = "Id-ul utilizatorului este obligatoriu.")]
    public string UserId { get; set; }
}

