using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.ChatJoinRequest;

public class CreateChatJoinRequestDto
{
    [Required(ErrorMessage = "Id-ul chat-ului este obligatoriu.")]
    public string ChatId { get; set; } = null!;
}
