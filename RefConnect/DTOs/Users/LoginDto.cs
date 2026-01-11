using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class LoginDto
{
    [Required(ErrorMessage = "Email este obligatoriu.")]
    [EmailAddress(ErrorMessage = "Email invalid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Parola este obligatorie.")]
    public string Password { get; set; }
}