using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class UpdateProfileDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    [Url]
    public string ProfileImageUrl { get; set; }

    [Required]
    public bool IsProfilePublic { get; set; }
}