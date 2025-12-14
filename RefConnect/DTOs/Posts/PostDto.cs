namespace RefConnect.DTOs.Posts;

public class PostDto
{
    public string PostId { get; set; }
    public string MediaType { get; set; }
    public string MediaUrl { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
}


