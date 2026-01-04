using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.ChatJoinRequest;

public class CreateChatJoinRequestDto
{
    [Required]
    public string ChatId { get; set; } = null!;
}
