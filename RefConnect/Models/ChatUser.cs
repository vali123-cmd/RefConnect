using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class ChatUser
{
    [Key]
    public int ChatUserId { get; set; }

    public int ChatId { get; set; }
    public virtual Chat Chat { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
}