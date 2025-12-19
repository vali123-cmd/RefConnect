namespace RefConnect.DTOs.Follows;


using System.ComponentModel.DataAnnotations;

public class FollowDTO
{
    [Required]
    public string FollowerId { get; set; }
    [Required]
    public string FollowingId { get; set; }
}