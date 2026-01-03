namespace RefConnect.DTOs.Likes;


using System.ComponentModel.DataAnnotations;
public class Like
{
    
    [Required]
    public string UserId { get; set; } = null!;


    
    [Required]
    public string PostId { get; set; } = null!;
 
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}