using RefConnect.DTOs.ChatJoinRequest;

namespace RefConnect.Services.Interfaces;

public interface IChatJoinRequestService
{
    /// <summary>
    /// Get all pending join requests for chats owned by the user
    /// </summary>
    Task<IEnumerable<ChatJoinRequestDto>> GetPendingRequestsForOwnerAsync(string ownerId, CancellationToken ct = default);
    
    /// <summary>
    /// Get all pending join requests for a specific chat (only accessible by chat owner)
    /// </summary>
    Task<IEnumerable<ChatJoinRequestDto>> GetPendingRequestsForChatAsync(string chatId, string requesterId, CancellationToken ct = default);
    
    /// <summary>
    /// Create a new join request for a group chat
    /// </summary>
    Task<ChatJoinRequestDto?> CreateJoinRequestAsync(string userId, string chatId, CancellationToken ct = default);
    
    /// <summary>
    /// Accept a join request (only accessible by chat owner)
    /// </summary>
    Task<bool> AcceptRequestAsync(string requestId, string ownerId, CancellationToken ct = default);
    
    /// <summary>
    /// Decline a join request (only accessible by chat owner)
    /// </summary>
    Task<bool> DeclineRequestAsync(string requestId, string ownerId, CancellationToken ct = default);
    
    /// <summary>
    /// Cancel a join request (only accessible by the requester)
    /// </summary>
    Task<bool> CancelRequestAsync(string requestId, string requesterId, CancellationToken ct = default);
    
    /// <summary>
    /// Get the user's own pending requests
    /// </summary>
    Task<IEnumerable<ChatJoinRequestDto>> GetUserPendingRequestsAsync(string userId, CancellationToken ct = default);
}
