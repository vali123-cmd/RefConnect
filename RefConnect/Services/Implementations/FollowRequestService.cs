

using RefConnect.Data;
using RefConnect.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RefConnect.Services.Interfaces;

namespace RefConnect.Services.Implementations;

public class FollowRequestService  :  IFollowRequestService
{
    private readonly ApplicationDbContext _dbContext;

    public FollowRequestService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> SendFollowRequestAsync(string followerId, string followingId, CancellationToken ct = default)
    {
        var existingRequest = await _dbContext.FollowRequests.FirstOrDefaultAsync(fr => fr.FollowerId == followerId && fr.FollowingId == followingId, ct);
        if (existingRequest != null)
        {
            return false;
        }
        var followRequest = new FollowRequest
        {
            FollowerId = followerId,
            FollowingId = followingId,
            RequestedAt = DateTime.UtcNow
        };
        _dbContext.FollowRequests.Add(followRequest);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> CancelFollowRequestAsync(string followerId, string followingId, CancellationToken ct = default)
    {
        var existingRequest = await _dbContext.FollowRequests.FirstOrDefaultAsync(fr => fr.FollowerId == followerId && fr.FollowingId == followingId, ct);
        if (existingRequest == null)
        {
            return false;
        }
        _dbContext.FollowRequests.Remove(existingRequest);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<FollowRequest>> GetPendingFollowRequestsAsync(string userId, CancellationToken ct = default)
    {
        return await _dbContext.FollowRequests
            .Where(fr => fr.FollowingId == userId)
            .ToListAsync(ct);
    }

    public async Task<bool> AcceptFollowRequestAsync(string followingId, string followerId, CancellationToken ct = default)
    {
        var existingRequest = await _dbContext.FollowRequests.FirstOrDefaultAsync(fr => fr.FollowerId == followerId && fr.FollowingId == followingId, ct);
        if (existingRequest == null)
        {
            return false;
        }
        var follow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId,
            FollowedAt = DateTime.UtcNow
        };
        _dbContext.Follows.Add(follow);
        _dbContext.FollowRequests.Remove(existingRequest);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeclineFollowRequestAsync(string followingId, string followerId, CancellationToken ct = default)
    {
        var existingRequest = await _dbContext.FollowRequests.FirstOrDefaultAsync(fr => fr.FollowerId == followerId && fr.FollowingId == followingId, ct);
        if (existingRequest == null)
        {
            return false;
        }
        _dbContext.FollowRequests.Remove(existingRequest);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }



  





    
}

public interface IFollowRequestService
{
    Task<bool> SendFollowRequestAsync(string requesterId, string targetUserId, CancellationToken ct = default);
    Task<bool> AcceptFollowRequestAsync(string targetUserId, string requesterId, CancellationToken ct = default);
    Task<bool> DeclineFollowRequestAsync(string targetUserId, string requesterId, CancellationToken ct = default);
    Task<bool> CancelFollowRequestAsync(string requesterId, string targetUserId, CancellationToken ct = default);
    Task<List<FollowRequest>> GetPendingFollowRequestsAsync(string userId, CancellationToken ct = default);
}

