using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)] // Enforce strong passwords here
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
    
        
    
    
}