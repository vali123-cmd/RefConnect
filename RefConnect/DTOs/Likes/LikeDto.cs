namespace RefConnect.DTOs.Likes;


using System.ComponentModel.DataAnnotations;



    public class LikeDto
    {
        public string UserId { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public DateTime? LikedAt { get; set; }
    }
