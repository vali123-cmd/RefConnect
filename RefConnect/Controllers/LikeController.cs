using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Likes;
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
        public async Task<IActionResult> LikePost(LikeDto like)
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
            var post = await _context.Posts.FindAsync(like.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }
            post.LikeCount += 1;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            

            like.LikedAt = DateTime.UtcNow;
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok("Post liked successfully.");
        }

        // DELETE: api/Like
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> UnlikePost(LikeDto like)
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
            var post = await _context.Posts.FindAsync(like.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }
            post.LikeCount -= 1;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return Ok("Post unliked successfully.");
        }
        [Authorize]
        [HttpGet("exists")]
        public async Task<bool> LikeExistsAsync(LikeExistsDto likeDto)
        {
            return await _context.Likes.AnyAsync(l => l.UserId == likeDto.UserId && l.PostId == likeDto.PostId);
        }
    }
}