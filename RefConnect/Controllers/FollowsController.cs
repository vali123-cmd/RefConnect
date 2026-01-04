using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Follow;
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
        
    }
};
