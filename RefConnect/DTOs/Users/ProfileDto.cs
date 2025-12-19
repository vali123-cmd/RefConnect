namespace RefConnect.DTOs.Users;
using RefConnect.DTOs.Posts;



public class ProfileDto
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Description { get; set; }
    public string ProfileImageUrl { get; set; }
    public bool IsProfilePublic { get; set; }

    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; } 

    public bool IsFollowing { get; set; } = false; // indicates if the current user is following this profile

    public List<PostDto> Posts { get; set; } = new();

}