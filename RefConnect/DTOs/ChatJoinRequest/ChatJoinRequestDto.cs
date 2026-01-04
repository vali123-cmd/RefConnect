namespace RefConnect.DTOs.ChatJoinRequest;

using System.ComponentModel.DataAnnotations;

public class ChatJoinRequestDto
{
    public string ChatJoinRequestId { get; set; } = null!;
    public string ChatId { get; set; } = null!;
    public string ChatName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? UserProfilePicture { get; set; }
    public string Status { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
}