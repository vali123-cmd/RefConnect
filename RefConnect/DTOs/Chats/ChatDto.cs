using RefConnect.DTOs.ChatUsers;
using RefConnect.DTOs.Messages;

namespace RefConnect.DTOs.Chats;

public class ChatDto
{
    public string ChatId { get; set; }
    public string ChatType { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string CreatedByUserId { get; set; }



    public ICollection<ChatUserDto> ChatUsers { get; set; }

    public ICollection<MessageDto> Messages { get; set; }

   
}


