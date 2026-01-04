using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Chats;

public class CreateGroupChatDto
{
    [Required]
    public string GroupName { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public List<string> InitialUserIds { get; set; } = new();
}
