using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Users;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "Prenumele este obligatoriu.")]
    [StringLength(100, ErrorMessage = "Prenumele poate avea maximum 100 de caractere.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Numele de familie este obligatoriu.")]
    [StringLength(100, ErrorMessage = "Numele de familie poate avea maximum 100 de caractere.")]
    public string LastName { get; set; }

    [StringLength(1000, ErrorMessage = "Descrierea poate avea maximum 1000 de caractere.")]
    public string? Description { get; set; }
    
    public string? ProfileImageUrl { get; set; }

    [Required(ErrorMessage = "Setarea de vizibilitate este obligatorie.")]
    public bool IsProfilePublic { get; set; }
}