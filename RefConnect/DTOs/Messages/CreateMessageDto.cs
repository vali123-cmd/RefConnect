using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Messages;

public class CreateMessageDto
{
    [Required(ErrorMessage = "Mesajul este obligatoriu.")]
    [StringLength(5000, ErrorMessage = "Mesajul poate avea maximum 5000 de caractere.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Id-ul chat-ului este obligatoriu.")]
    public string ChatId { get; set; }

    [Required(ErrorMessage = "Id-ul utilizatorului este obligatoriu.")]
    public string UserId { get; set; }
}

