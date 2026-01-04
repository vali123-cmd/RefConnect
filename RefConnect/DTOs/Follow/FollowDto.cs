using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Follow;

public class FollowDto
{
    [Required]
    public string FollowerId { get; set; }
    [Required]
    public string FollowingId { get; set; }
    public DateTime FollowedAt { get; set; }
}