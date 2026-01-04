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
        public async Task<IActionResult> LikePost([FromBody] LikeDto likeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != likeDto.UserId && !isAdmin)
                return Forbid();

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == likeDto.UserId && l.PostId == likeDto.PostId);
            if (existingLike != null)
                return Conflict(new { message = "Already liked." });

            var post = await _context.Posts.FindAsync(likeDto.PostId);
            if (post == null)
                return NotFound(new { message = "Post not found." });

            post.LikeCount += 1;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();


            var likeEntity = new Like
            {
                UserId = likeDto.UserId,
                PostId = likeDto.PostId,
                LikedAt = likeDto.LikedAt ?? DateTime.UtcNow
            };

            _context.Likes.Add(likeEntity);
            await _context.SaveChangesAsync();

            // return DTO to avoid serializing navigation properties and cycles
            var resultDto = new LikeDto
            {
                UserId = likeEntity.UserId,
                PostId = likeEntity.PostId,
                LikedAt = likeEntity.LikedAt
            };

            return Ok(resultDto);
        }

        // DELETE: api/Like
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> UnlikePost([FromBody] LikeDto likeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (userId != likeDto.UserId && !isAdmin)
                return Forbid();

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == likeDto.UserId && l.PostId == likeDto.PostId);
            if (existingLike == null)
                return NotFound(new { message = "Like not found." });
            var post = await _context.Posts.FindAsync(likeDto.PostId);
            if (post == null)
                return NotFound(new { message = "Post not found." });
            post.LikeCount = Math.Max(0, post.LikeCount - 1);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize]
        [HttpPost("exists")]
        public async Task<ActionResult<bool>> LikeExistsAsync([FromBody] LikeExistsDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.PostId))
                return BadRequest(new { error = "userId and postId are required." });

            var exists = await _context.Likes.AnyAsync(l => l.UserId == req.UserId && l.PostId == req.PostId);
            return Ok(exists);
        }
    }
}