namespace RefConnect.Models;




public class FollowRequest
{
    public string FollowerId { get; set; }
    public virtual ApplicationUser FollowerRequest { get; set; }

    public string FollowingId { get; set; }
    public virtual ApplicationUser FollowingRequest { get; set; }

    public DateTime RequestedAt { get; set; }
}