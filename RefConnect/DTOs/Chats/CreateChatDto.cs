using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Chats;

public class CreateChatDto
{
    [Required]
    public string ChatType { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public string? MatchId { get; set; }
}

