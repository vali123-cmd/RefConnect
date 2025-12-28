using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefConnect.Models;




using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RefConnect.DTOs.Chats;
using RefConnect.DTOs.Messages;
using RefConnect.DTOs.Shared;

namespace RefConnect.Services.Interfaces;
public interface IFollowRequest
{
    Task<bool> SendFollowRequestAsync(string requesterId, string targetUserId, CancellationToken ct = default);
    Task<bool> AcceptFollowRequestAsync(string targetUserId, string requesterId, CancellationToken ct = default);
    Task<bool> DeclineFollowRequestAsync(string targetUserId, string requesterId, CancellationToken ct = default);
    Task<bool> CancelFollowRequestAsync(string requesterId, string targetUserId, CancellationToken ct = default);
    Task<IEnumerable<string>> GetPendingFollowRequestsAsync(string userId, CancellationToken ct = default);
}