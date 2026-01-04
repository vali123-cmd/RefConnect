using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Chats;

public class UpdateChatDto
{
    public string? ChatName { get; set; }
    
    public string? Description { get; set; }

    public string? ChatType { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool? IsActive { get; set; }

    public string? MatchId { get; set; }
}

