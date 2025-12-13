using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}