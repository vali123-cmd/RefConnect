using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class ProfileDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FullName { get; set; }

   
    [StringLength(1000)]
    public string Description { get; set; }

    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
  
    [Url]
    public string ProfileImageUrl { get; set; }
    [Required]
    public bool IsProfilePublic { get; set; }
}