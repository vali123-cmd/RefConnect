using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;
public class Follow
{
    
    [Required]
    public string FollowerId { get; set; }
    public virtual ApplicationUser Follower { get; set; }

    [Required]
    public string FollowingId { get; set; }
    public virtual ApplicationUser Following { get; set; }

    public DateTime FollowedAt { get; set; }
}