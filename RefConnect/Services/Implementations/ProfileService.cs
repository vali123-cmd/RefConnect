namespace RefConnect.Services.Implementations;


using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.DTOs.Users;
using RefConnect.Models;
using RefConnect.DTOs.Posts;
using RefConnect.DTOs.MatchAssigments;
using RefConnect.Services.Interfaces;



public class ProfileService 
{
    private readonly ApplicationDbContext _dbContext;
    public ProfileService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<ProfileDto>> SearchUsersAsync(string query, int limit = 20, CancellationToken ct = default)
    {

        return await _dbContext.Users.OfType<ApplicationUser>()
            .Where(u => u.UserName.Contains(query))
            .Take(limit)
            .Select(u => new ProfileDto
            {
                
                UserName = u.UserName,
                FullName = $"{u.FirstName} {u.LastName}",
                Description = u.Description,
                ProfileImageUrl = u.ProfileImageUrl,
                IsProfilePublic = u.IsProfilePublic,
                FollowersCount = u.FollowersCount,
                FollowingCount = u.FollowingCount,
            })
            .ToListAsync(ct);
    }
    //in controller se va folosi aceasta functie pentru a stabili daca requester-ul are voie sa vada datele extinse ale profilului
    public async Task<bool> mayViewProfileExtendedAsync(string userId, string requesterId, CancellationToken ct = default)
    {
        var isProfilePublic = await _dbContext.Users.OfType<ApplicationUser>()
            .Where(u => u.Id == userId)
            .Select(u => u.IsProfilePublic)
            .FirstOrDefaultAsync(ct);

        if (isProfilePublic)
        {
            return true;
        }
        else if(requesterId == userId)
        {
            return true;
        }
        else 
        {
            var isFollowing = await _dbContext.Follows
                .AnyAsync(f => f.FollowerId == requesterId && f.FollowingId == userId, ct);
            return isFollowing;
        }

    }
    public async Task<ProfileDto?> GetProfileAsync(string userId, string? requesterId = null, CancellationToken ct = default)
    {
        var isProfilePublic = await _dbContext.Users.OfType<ApplicationUser>()
            .Where(u => u.Id == userId)
            .Select(u => u.IsProfilePublic)
            .FirstOrDefaultAsync(ct);
        
       
           
            return await _dbContext.Users.OfType<ApplicationUser>()
                .Where(u => u.Id == userId)
                .Select(u => new ProfileDto
                {
                    UserName = u.UserName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Description = u.Description,
                    ProfileImageUrl = u.ProfileImageUrl,
                    IsProfilePublic = u.IsProfilePublic,
                    FollowersCount = u.FollowersCount,
                    FollowingCount = u.FollowingCount,
                })
                .FirstOrDefaultAsync(ct);

            

       
    }
    public async Task<ProfileExtendedDto?> GetProfileExtendedAsync(string userId, string requesterId, CancellationToken ct = default)
    {
        var canView = await mayViewProfileExtendedAsync(userId, requesterId, ct);
        if (!canView)
        {
            return null;
        }

        return await _dbContext.Users.OfType<ApplicationUser>()
            .Where(u => u.Id == userId)
            .Select(u => new ProfileExtendedDto
            {
                UserName = u.UserName,
                FullName = $"{u.FirstName} {u.LastName}",
                Description = u.Description,
                ProfileImageUrl = u.ProfileImageUrl,
                IsProfilePublic = u.IsProfilePublic,
                FollowersCount = u.FollowersCount,
                FollowingCount = u.FollowingCount,
                FollowersIds = u.Followers.Select(f => f.FollowerId).ToList(),
                FollowingIds = u.Following.Select(f => f.FollowingId).ToList(),
                Posts = u.Posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Description = p.Description,
                    MediaType = p.MediaType,
                    MediaUrl = p.MediaUrl,
                    CreatedAt = p.CreatedAt

                }).ToList(),
                MatchAssignments = u.MatchAssignments.Select(ma => new MatchAssignmentDto
                {
                    MatchAssignmentId = ma.MatchAssignmentId,
                    RoleInMatch = ma.RoleInMatch,
                    UserId = ma.UserId,
                    MatchId = ma.MatchId
                    
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

            //pentru utilizator returnez deja datele despre postari in json, pentru a evita multiple apeluri catre server
            /* pentru meciuri(care pot fi multe si nu sunt mereu necesare) nu le includ in acest endpoint, ci le las pentru un endpoint separat, deoarece
            / exista multe date legate de meciuri care pot incarca mult raspunsul si nu sunt intotdeauna necesare */
    }



}
