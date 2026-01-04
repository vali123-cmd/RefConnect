using System.ComponentModel.DataAnnotations;
using RefConnect.DTOs.Posts;
using RefConnect.DTOs.MatchAssigments;


namespace RefConnect.DTOs.Users;

public class ProfileExtendedDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string FullName { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

  
    public string? ProfileImageUrl { get; set; }

    [Required]
    public bool IsProfilePublic { get; set; }

    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }

    [Required]
    public ICollection<string> FollowersIds { get; set; }
    [Required]
    public ICollection<string> FollowingIds { get; set; }

    [Required]
    public ICollection<PostDto> Posts { get; set; } 

    [Required]
    public ICollection<MatchAssignmentDto> MatchAssignments { get; set; }


    
}