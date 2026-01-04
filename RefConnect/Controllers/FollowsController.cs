using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Follow;
using RefConnect.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;




namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Follows
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FollowUser([FromBody] FollowDto followDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != followDto.FollowerId && !isAdmin)
            {
                return Forbid();
            }

            if (followDto.FollowerId == followDto.FollowingId)
            {
                return BadRequest("You cannot follow yourself.");
            }

            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followDto.FollowerId && f.FollowingId == followDto.FollowingId);

            if (existingFollow != null)
            {
                return Conflict("You are already following this user.");
            }

            //if the profile is private, a follow request should be sent instead
            
            var follow = new Follow
            {
                FollowerId = followDto.FollowerId,
                FollowingId = followDto.FollowingId,
                FollowedAt = DateTime.UtcNow
            };
            
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // DELETE: api/Follows
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowDto followDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != followDto.FollowerId && !isAdmin)
            {
                return Forbid();
            }
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followDto.FollowerId && f.FollowingId == followDto.FollowingId);
            if (existingFollow == null)
            {
                return NotFound("You are not following this user.");

            }
            _context.Follows.Remove(existingFollow);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Follows/{userId}/followers
        [HttpGet("{userId}/followers")]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetFollowers(string userId)
        {
            var followers = await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Include(f => f.Follower)
                .Select(f => new ProfileDto
                {
                    Id = f.Follower.Id,
                    UserName = f.Follower.UserName,
                    FullName = $"{f.Follower.FirstName} {f.Follower.LastName}",
                    Description = f.Follower.Description,
                    ProfileImageUrl = f.Follower.ProfileImageUrl,
                    IsProfilePublic = f.Follower.IsProfilePublic,
                    FollowersCount = f.Follower.FollowersCount,
                    FollowingCount = f.Follower.FollowingCount
                })
                .ToListAsync();

            return Ok(followers);
        }

        // GET: api/Follows/{userId}/following
        [HttpGet("{userId}/following")]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetFollowing(string userId)
        {
            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Following)
                .Select(f => new ProfileDto
                {
                    Id = f.Following.Id,
                    UserName = f.Following.UserName,
                    FullName = $"{f.Following.FirstName} {f.Following.LastName}",
                    Description = f.Following.Description,
                    ProfileImageUrl = f.Following.ProfileImageUrl,
                    IsProfilePublic = f.Following.IsProfilePublic,
                    FollowersCount = f.Following.FollowersCount,
                    FollowingCount = f.Following.FollowingCount
                })
                .ToListAsync();

            return Ok(following);
        }
        
    }
};
