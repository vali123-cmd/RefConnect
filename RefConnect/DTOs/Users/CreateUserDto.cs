using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)] 
    public string Password { get; set; }

    [Url]
    public string ProfileImageUrl { get; set; }
    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    public string FirstName { get; set; }

 
    
    [Required]
    public string LastName { get; set; }
    
        
    
    
}