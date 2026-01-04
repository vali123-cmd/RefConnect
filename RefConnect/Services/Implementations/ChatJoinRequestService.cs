using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.DTOs.ChatJoinRequest;
using RefConnect.Models;
using RefConnect.Services.Interfaces;

namespace RefConnect.Services.Implementations;

public class ChatJoinRequestService : IChatJoinRequestService
{
    private readonly ApplicationDbContext _context;

    public ChatJoinRequestService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatJoinRequestDto>> GetPendingRequestsForOwnerAsync(string ownerId, CancellationToken ct = default)
    {
        var requests = await _context.ChatJoinRequests
            .Include(r => r.Chat)
            .Include(r => r.User)
            .Where(r => r.Chat.CreatedByUserId == ownerId && r.Status == "Pending")
            .OrderByDescending(r => r.RequestedAt)
            .Select(r => new ChatJoinRequestDto
            {
                ChatJoinRequestId = r.ChatJoinRequestId,
                ChatId = r.ChatId,
                ChatName = r.Chat.Description ?? "Group Chat",
                UserId = r.UserId,
                UserName = r.User.UserName ?? "",
                UserProfilePicture = r.User.ProfileImageUrl,
                Status = r.Status,
                RequestedAt = r.RequestedAt
            })
            .ToListAsync(ct);

        return requests;
    }

    public async Task<IEnumerable<ChatJoinRequestDto>> GetPendingRequestsForChatAsync(string chatId, string requesterId, CancellationToken ct = default)
    {
        // Verify the requester is the chat owner
        var chat = await _context.Chats.FindAsync(new object[] { chatId }, ct);
        if (chat == null || chat.CreatedByUserId != requesterId)
        {
            return Enumerable.Empty<ChatJoinRequestDto>();
        }

        var requests = await _context.ChatJoinRequests
            .Include(r => r.User)
            .Where(r => r.ChatId == chatId && r.Status == "Pending")
            .OrderByDescending(r => r.RequestedAt)
            .Select(r => new ChatJoinRequestDto
            {
                ChatJoinRequestId = r.ChatJoinRequestId,
                ChatId = r.ChatId,
                ChatName = chat.Description ?? "Group Chat",
                UserId = r.UserId,
                UserName = r.User.UserName ?? "",
                UserProfilePicture = r.User.ProfileImageUrl,
                Status = r.Status,
                RequestedAt = r.RequestedAt
            })
            .ToListAsync(ct);

        return requests;
    }

    public async Task<ChatJoinRequestDto?> CreateJoinRequestAsync(string userId, string chatId, CancellationToken ct = default)
    {
        // Verify chat exists and is a group chat
        var chat = await _context.Chats
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.ChatId == chatId, ct);

        if (chat == null || chat.ChatType != "group")
        {
            return null;
        }

        // Check if user is already a member
        if (chat.ChatUsers.Any(cu => cu.UserId == userId))
        {
            return null;
        }

        // Check if there's already a pending request
        var existingRequest = await _context.ChatJoinRequests
            .FirstOrDefaultAsync(r => r.ChatId == chatId && r.UserId == userId && r.Status == "Pending", ct);

        if (existingRequest != null)
        {
            return null;
        }

        var user = await _context.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
        {
            return null;
        }

        var request = new ChatJoinRequest
        {
            ChatJoinRequestId = Guid.NewGuid().ToString(),
            ChatId = chatId,
            UserId = userId,
            Status = "Pending",
            RequestedAt = DateTime.UtcNow
        };

        _context.ChatJoinRequests.Add(request);
        await _context.SaveChangesAsync(ct);

        return new ChatJoinRequestDto
        {
            ChatJoinRequestId = request.ChatJoinRequestId,
            ChatId = request.ChatId,
            ChatName = chat.Description ?? "Group Chat",
            UserId = request.UserId,
            UserName = user.UserName ?? "",
            UserProfilePicture = user.ProfileImageUrl,
            Status = request.Status,
            RequestedAt = request.RequestedAt
        };
    }

    public async Task<bool> AcceptRequestAsync(string requestId, string ownerId, CancellationToken ct = default)
    {
        var request = await _context.ChatJoinRequests
            .Include(r => r.Chat)
            .FirstOrDefaultAsync(r => r.ChatJoinRequestId == requestId, ct);

        if (request == null || request.Status != "Pending")
        {
            return false;
        }

        // Verify the owner is the chat creator
        if (request.Chat.CreatedByUserId != ownerId)
        {
            return false;
        }

        // Add user to chat
        var chatUser = new ChatUser
        {
            ChatUserId = Guid.NewGuid().ToString(),
            ChatId = request.ChatId,
            UserId = request.UserId
        };

        _context.ChatUsers.Add(chatUser);
        
        // Update request status
        request.Status = "Accepted";
        
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeclineRequestAsync(string requestId, string ownerId, CancellationToken ct = default)
    {
        var request = await _context.ChatJoinRequests
            .Include(r => r.Chat)
            .FirstOrDefaultAsync(r => r.ChatJoinRequestId == requestId, ct);

        if (request == null || request.Status != "Pending")
        {
            return false;
        }

        // Verify the owner is the chat creator
        if (request.Chat.CreatedByUserId != ownerId)
        {
            return false;
        }

        request.Status = "Declined";
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CancelRequestAsync(string requestId, string requesterId, CancellationToken ct = default)
    {
        var request = await _context.ChatJoinRequests
            .FirstOrDefaultAsync(r => r.ChatJoinRequestId == requestId && r.UserId == requesterId, ct);

        if (request == null || request.Status != "Pending")
        {
            return false;
        }

        _context.ChatJoinRequests.Remove(request);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<ChatJoinRequestDto>> GetUserPendingRequestsAsync(string userId, CancellationToken ct = default)
    {
        var requests = await _context.ChatJoinRequests
            .Include(r => r.Chat)
            .Where(r => r.UserId == userId && r.Status == "Pending")
            .OrderByDescending(r => r.RequestedAt)
            .Select(r => new ChatJoinRequestDto
            {
                ChatJoinRequestId = r.ChatJoinRequestId,
                ChatId = r.ChatId,
                ChatName = r.Chat.Description ?? "Group Chat",
                UserId = r.UserId,
                UserName = "",
                UserProfilePicture = null,
                Status = r.Status,
                RequestedAt = r.RequestedAt
            })
            .ToListAsync(ct);

        return requests;
    }
}
