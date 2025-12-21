

using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class UserDto
{
    [Required]
    public string Id { get; set; } 
    [Required]
    public string Email { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]

    public string UserName { get; set; }
    [Required]
    public string FullName => $"{FirstName} {LastName}"; 
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public string ProfileImageUrl { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool IsProfilePublic { get; set; }
    [Required]
    public int FollowersCount { get; set; }
    [Required]

    public int FollowingCount { get; set; }
    
    
    
}

