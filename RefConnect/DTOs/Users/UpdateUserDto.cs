namespace RefConnect.DTOs.Users;
using System.ComponentModel.DataAnnotations;



public class UpdateUserDto
{

    
    [Required]
    [StringLength(100)]
    public string UserName { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; }

    
    
    public string? ProfileImageUrl { get; set; }

    [Required]
    public bool IsProfilePublic { get; set; }
    
    
    
    
    
}