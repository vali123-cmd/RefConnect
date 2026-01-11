using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class CreateUserDto
{
    [Required(ErrorMessage = "Email este obligatoriu.")]
    [EmailAddress(ErrorMessage = "Email invalid.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Numele de utilizator este obligatoriu.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Numele de utilizator trebuie să aibă între 3 și 50 de caractere.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Parola este obligatorie.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola trebuie să aibă cel puțin 6 caractere.")]
    public string Password { get; set; }

    public string? ProfileImageUrl { get; set; }

    [StringLength(1000, ErrorMessage = "Descrierea poate avea maximum 1000 de caractere.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Prenumele este obligatoriu.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Numele de familie este obligatoriu.")]
    public string LastName { get; set; }
    
        
    
    
}