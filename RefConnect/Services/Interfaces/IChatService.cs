using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefConnect.DTOs.Chats;
using RefConnect.DTOs.Messages;
using RefConnect.DTOs.Shared;

namespace RefConnect.Services.Interfaces;

public interface IChatService
{
    // Create or return existing direct chat between two users
    Task<IEnumerable<ChatDto>> GetChatsForUserAsync(string userId, CancellationToken ct = default);
    Task<ChatDto> CreateDirectChatAsync(string userAId, string userBId, CancellationToken ct = default);

    // Create a new group chat with initial members (creator will be added as member)
    Task<ChatDto> CreateGroupChatAsync(string creatorId, string groupName, IEnumerable<string> initialUserIds, CancellationToken ct = default);

    // Add/Remove members (group only)
    Task<bool> AddUserToGroupAsync(string chatId, string requesterId, string userIdToAdd, CancellationToken ct = default);
    Task<bool> RemoveUserFromGroupAsync(string chatId, string requesterId, string userIdToRemove, CancellationToken ct = default);

    // Send a message in a chat
    Task<MessageDto> SendMessageAsync(CreateMessageDto dto, CancellationToken ct = default);

    // Get paged messages (enforces membership)
    Task<PagedResult<MessageDto>> GetMessagesAsync(string chatId, string? requesterId, bool isAdmin, int page = 1, int pageSize = 50, CancellationToken ct = default);

    // Additional utilities

    Task<bool> LeaveChatAsync(string chatId, string userId, CancellationToken ct = default);
}
