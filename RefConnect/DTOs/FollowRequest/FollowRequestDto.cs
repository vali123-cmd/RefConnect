
using System;
using System.ComponentModel.DataAnnotations;


namespace RefConnect.DTOs.FollowRequest;
public class FollowRequestDto
{
    [Required]
    public string FollowRequestId { get; set; }
    [Required]
    public string FollowerId { get; set; }
    [Required]
    public string FollowingId { get; set; }
    [Required]
    public DateTime RequestedAt { get; set; }
}