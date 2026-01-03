using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Like
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost([FromBody] Like like)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != like.UserId && !isAdmin)
            {
                return Forbid();
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == like.UserId && l.PostId == like.PostId);

            if (existingLike != null)
            {
                return Conflict("You have already liked this post.");
            }

            like.LikedAt = DateTime.UtcNow;
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok("Post liked successfully.");
        }

        // DELETE: api/Like
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> UnlikePost([FromBody] Like like)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != like.UserId && !isAdmin)
            {
                return Forbid("You are not authorized to unlike this post in name of another user.");

            }
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == like.UserId && l.PostId == like.PostId);
            if (existingLike == null)
            {
                return NotFound("Like not found.");
            }
            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return Ok("Post unliked successfully.");
        }
    }
}