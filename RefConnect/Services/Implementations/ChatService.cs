using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.DTOs.Chats;
using RefConnect.DTOs.ChatUsers;
using RefConnect.DTOs.Messages;
using RefConnect.DTOs.Shared;
using RefConnect.Models;
using RefConnect.Services.Interfaces;


namespace RefConnect.Services.Implementations;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _db;

    public ChatService(ApplicationDbContext db)
    {
        _db = db;
    }

   

    public async Task<ChatDto> CreateGroupChatAsync(string creatorId, string groupName, IEnumerable<string> initialUserIds, CancellationToken ct = default)
    {
        var safeName = string.IsNullOrWhiteSpace(groupName) ? "New Group" : groupName.Trim();

        var newChat = new Chat
        {
            ChatId = Guid.NewGuid().ToString(),
            ChatType = "group",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = creatorId,
            Name = safeName,
            Description = safeName
        };

        var memberIds = new HashSet<string>(initialUserIds ?? Enumerable.Empty<string>());
        memberIds.Add(creatorId);

        foreach (var userId in memberIds.Where(id => !string.IsNullOrWhiteSpace(id)))
        {
            newChat.ChatUsers.Add(new ChatUser
            {
                ChatUserId = Guid.NewGuid().ToString(),
                ChatId = newChat.ChatId,
                UserId = userId
            });
        }

        _db.Chats.Add(newChat);
        await _db.SaveChangesAsync(ct);

        return new ChatDto
        {
            ChatId = newChat.ChatId,
            ChatType = newChat.ChatType,
            CreatedAt = newChat.CreatedAt,
            CreatedByUserId = newChat.CreatedByUserId,
            Name = newChat.Name,
            Description = newChat.Description,
            

        };
    }

    public async Task<ChatDto> CreateGroupChatAsync(string creatorId, CreateGroupChatDto dto, CancellationToken ct = default)
    {
        return await CreateGroupChatAsync(creatorId, dto.GroupName, dto.InitialUserIds, ct);
    }

    public async Task<ChatDto?> CreateDirectChatAsync(string userAId, string userBId, CancellationToken ct = default)
    {
        // Check if a direct chat already exists between the two users
        var existingChat = await _db.Chats
            .Include(c => c.ChatUsers)
            .Where(c => c.ChatType == "direct")
            .Where(c => c.ChatUsers.Any(cu => cu.UserId == userAId) && c.ChatUsers.Any(cu => cu.UserId == userBId))
            .FirstOrDefaultAsync(ct);

        if (existingChat != null)
        {
            return new ChatDto
            {
                ChatId = existingChat.ChatId,
                ChatType = existingChat.ChatType,
                CreatedAt = existingChat.CreatedAt,
                CreatedByUserId = existingChat.CreatedByUserId,
                Name = existingChat.Name,
                Description = existingChat.Description
            };
        }

        var newChat = new Chat
        {
            ChatId = Guid.NewGuid().ToString(),
            ChatType = "direct",
            CreatedAt = DateTime.UtcNow,

            CreatedByUserId = userAId,
            Name = "Direct Chat",
            Description = "Direct Chat"
        };

        newChat.ChatUsers.Add(new ChatUser { ChatUserId = Guid.NewGuid().ToString(), ChatId = newChat.ChatId, UserId = userAId });
        newChat.ChatUsers.Add(new ChatUser { ChatUserId = Guid.NewGuid().ToString(), ChatId = newChat.ChatId, UserId = userBId });

        _db.Chats.Add(newChat);
        await _db.SaveChangesAsync(ct);

        return new ChatDto
        {
            ChatId = newChat.ChatId,
            ChatType = newChat.ChatType,
            Name = newChat.Name,
            CreatedAt = newChat.CreatedAt,
            CreatedByUserId = newChat.CreatedByUserId,
            Description = newChat.Description
        };
    }

    public async Task<bool> AddUserToGroupAsync(string chatId, string requesterId, string userIdToAdd, CancellationToken ct = default)
    {
        var chat = await _db.Chats.Include(c => c.ChatUsers).FirstOrDefaultAsync(c => c.ChatId == chatId, ct);
        if (chat == null) return false;
        if (chat.ChatType != "group") return false;

        var isMember = chat.ChatUsers.Any(cu => cu.UserId == requesterId);
        if (!isMember) return false;

        if (chat.ChatUsers.Any(cu => cu.UserId == userIdToAdd)) return true;

        chat.ChatUsers.Add(new ChatUser { ChatUserId = Guid.NewGuid().ToString(), ChatId = chatId, UserId = userIdToAdd });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveUserFromGroupAsync(string chatId, string requesterId, string userIdToRemove, CancellationToken ct = default)
    {
        var chat = await _db.Chats.Include(c => c.ChatUsers).FirstOrDefaultAsync(c => c.ChatId == chatId, ct);
        if (chat == null) return false;
        if (chat.ChatType != "group") return false;

        var isMember = chat.ChatUsers.Any(cu => cu.UserId == requesterId);
        if (!isMember) return false;

        var toRemove = chat.ChatUsers.FirstOrDefault(cu => cu.UserId == userIdToRemove);
        if (toRemove == null) return false;

        _db.ChatUsers.Remove(toRemove);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<MessageDto> SendMessageAsync(CreateMessageDto dto, CancellationToken ct = default)
    {
        var chat = await _db.Chats.Include(c => c.ChatUsers).FirstOrDefaultAsync(c => c.ChatId == dto.ChatId, ct);
        if (chat == null) throw new InvalidOperationException("Chat not found");

        var isMember = chat.ChatUsers.Any(cu => cu.UserId == dto.UserId);
        if (!isMember) throw new UnauthorizedAccessException("User is not a member of the chat");

        var message = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            Content = dto.Content,
            SentAt = DateTime.UtcNow,
            ChatId = dto.ChatId,
            UserId = dto.UserId
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(ct);

        return new MessageDto
        {
            MessageId = message.MessageId,
            Content = message.Content,
            SentAt = message.SentAt,
            ChatId = message.ChatId,
            UserId = message.UserId
        };
    }

    public async Task<PagedResult<MessageDto>> GetMessagesAsync(string chatId, string? requesterId, bool isAdmin, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        pageSize = Math.Clamp(pageSize, 1, 200);

        if (!isAdmin)
        {
            if (string.IsNullOrEmpty(requesterId)) throw new UnauthorizedAccessException("Authentication required");

            var isMember = await _db.ChatUsers.AsNoTracking().AnyAsync(cu => cu.ChatId == chatId && cu.UserId == requesterId, ct);
            if (!isMember) throw new UnauthorizedAccessException("Not a member of this chat");
        }

        var baseQuery = _db.Messages.AsNoTracking().Where(m => m.ChatId == chatId);
        var total = await baseQuery.LongCountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                Content = m.Content,
                SentAt = m.SentAt,
                ChatId = m.ChatId,
                UserId = m.UserId
            })
            .ToListAsync(ct);

        return new PagedResult<MessageDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    

    public async Task<bool> LeaveChatAsync(string chatId, string userId, CancellationToken ct = default)
    {
        var cu = await _db.ChatUsers.FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserId == userId, ct);
        if (cu == null) return false;
        _db.ChatUsers.Remove(cu);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteChatAsync(string userId, string chatId, bool isAdmin = false, CancellationToken ct = default)
    {
        var chat = await _db.Chats.FirstOrDefaultAsync(c => c.ChatId == chatId, ct);
        if (chat == null) return false;
        
        // Only the creator or admin can delete the chat
        if (!isAdmin && chat.CreatedByUserId != userId) return false;
        
        _db.Chats.Remove(chat);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateChatAsync(string userId, string chatId, UpdateChatDto dto, bool isAdmin = false, CancellationToken ct = default)
    {
        var chat = await _db.Chats.FirstOrDefaultAsync(c => c.ChatId == chatId, ct);
        if (chat == null) return false;
        
        // Only the creator or admin can update the chat
        if (!isAdmin && chat.CreatedByUserId != userId) return false;

        // Update name/description using the DTO's actual fields
        if (!string.IsNullOrWhiteSpace(dto.ChatName))
            chat.Name = dto.ChatName;

        if (dto.Description != null)
            chat.Description = dto.Description;

        if (!string.IsNullOrWhiteSpace(dto.ChatType))
            chat.ChatType = dto.ChatType;

        if (dto.MatchId != null)
            chat.MatchId = dto.MatchId;
        
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<ChatDto>> GetChatsForUserAsync(string userId, CancellationToken ct = default)
    {
        var chats = await _db.ChatUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.Chat)
            .Select(c => new ChatDto
            {
                ChatId = c.ChatId,
                ChatType = c.ChatType,
                CreatedAt = c.CreatedAt,
                Name = c.Name,
                CreatedByUserId = c.CreatedByUserId,
                Description = c.Description,
                ChatUsers = c.ChatUsers.Select(cu => new ChatUserDto
                {
                    ChatUserId = cu.ChatUserId,
                    ChatId = cu.ChatId,
                    UserId = cu.UserId
                }).ToList()
                })
            .ToListAsync(ct);

        return chats;
    }

    public async Task<IEnumerable<ChatDto>> GetAllChatsAsync(string? chatType = null, CancellationToken ct = default)
    {
        var query = _db.Chats
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(chatType))
        {
            query = query.Where(c => c.ChatType == chatType);
        }

        var chats = await query
            .Select(c => new ChatDto
            {
                ChatId = c.ChatId,
                ChatType = c.ChatType,
                CreatedAt = c.CreatedAt,
                Name = c.Name,
                CreatedByUserId = c.CreatedByUserId,
                Description = c.Description,
                ChatUsers = c.ChatUsers.Select(cu => new ChatUserDto
                {
                    ChatUserId = cu.ChatUserId,
                    ChatId = cu.ChatId,
                    UserId = cu.UserId
                }).ToList()
            })
            .ToListAsync(ct);

        return chats;
    }
    public async Task<IEnumerable<ChatDto>> SearchChatsAsync(string requesterId, string query, CancellationToken ct = default)
    {
        var chats = await _db.ChatUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == requesterId && cu.Chat.Description!.Contains(query))
            .Select(cu => cu.Chat)
            .Select(c => new ChatDto
            {
                ChatId = c.ChatId,
                ChatType = c.ChatType,
                CreatedAt = c.CreatedAt,
                CreatedByUserId = c.CreatedByUserId,
                Name = c.Name,
                Description = c.Description,
                ChatUsers = c.ChatUsers.Select(cu => new ChatUserDto
                {
                    ChatUserId = cu.ChatUserId,
                    ChatId = cu.ChatId,
                    UserId = cu.UserId
                }).ToList()
            })
            .ToListAsync(ct);

        return chats;
    }

    public async Task<IEnumerable<ChatDto>> SearchChatsAsync(
        string? requesterId,
        string query,
        bool isAdmin = false,
        string? chatType = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<ChatDto>();
        }

        var q = query.Trim();

        var chatsQuery = _db.Chats
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(chatType))
        {
            chatsQuery = chatsQuery.Where(c => c.ChatType == chatType);
        }

        // If not admin, restrict to chats the requester is a member of.
        if (!isAdmin)
        {
            if (string.IsNullOrWhiteSpace(requesterId))
                return Array.Empty<ChatDto>();

            chatsQuery = chatsQuery.Where(c => c.ChatUsers.Any(cu => cu.UserId == requesterId));
        }

        // Search by name/description (EF will translate to LIKE)
        chatsQuery = chatsQuery.Where(c =>
            c.Name.Contains(q) ||
            c.Description.Contains(q));

        var results = await chatsQuery
            .Select(c => new ChatDto
            {
                ChatId = c.ChatId,
                ChatType = c.ChatType,
                CreatedAt = c.CreatedAt,
                Name = c.Name,
                CreatedByUserId = c.CreatedByUserId,
                Description = c.Description,
                ChatUsers = c.ChatUsers.Select(cu => new ChatUserDto
                {
                    ChatUserId = cu.ChatUserId,
                    ChatId = cu.ChatId,
                    UserId = cu.UserId
                }).ToList()
            })
            .ToListAsync(ct);

        return results;
    }
}
