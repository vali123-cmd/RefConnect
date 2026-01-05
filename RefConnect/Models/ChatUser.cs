using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class ChatUser
{
    [Key]
    public string ChatUserId { get; set; }

    public string ChatId { get; set; }
    public virtual Chat Chat { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}