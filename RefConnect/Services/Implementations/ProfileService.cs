namespace RefConnect.Services.Implementations;


using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.DTOs.Users;
using RefConnect.Models;
using RefConnect.Services.Interfaces;


public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _dbContext;
    public ProfileService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<ProfileDto>> SearchUsersAsync(string query, int limit = 20, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Where(u => u.UserName.Contains(query))
            .Take(limit)
            .Select(u => new ProfileDto
            {
                
                UserName = u.UserName,
                FullName = $"{u.FirstName} {u.LastName}",
                Description = u.Description,
                ProfileImageUrl = u.ProfileImageUrl,
                IsProfilePublic = u.IsProfilePublic,
                FollowersCount = u.Followers.Count,
                FollowingCount = u.Following.Count,
                IsFollowing = false // cannot determine without requester context
            })
            .ToListAsync(ct);
    }

}
