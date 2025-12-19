namespace RefConnect.Models;


public class Follow
{
    // composite key will be configured in DbContext
    public string FollowerId { get; set; }
    public ApplicationUser Follower { get; set; }

    public string FollowingId { get; set; }
    public ApplicationUser Following { get; set; }

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    
}