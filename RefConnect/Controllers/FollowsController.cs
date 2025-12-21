using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;



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
        [HttpPost]
        public async Task<IActionResult> FollowUser([FromBody] Follow follow)
        {
            if (follow.FollowerId == follow.FollowingId)
            {
                return BadRequest("You cannot follow yourself.");
            }

            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == follow.FollowerId && f.FollowingId == follow.FollowingId);

            if (existingFollow != null)
            {
                return Conflict("You are already following this user.");
            }

            follow.FollowedAt = DateTime.UtcNow;
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // DELETE: api/Follows
        [HttpDelete]
        public async Task<IActionResult> UnfollowUser([FromBody] Follow follow)
        {
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == follow.FollowerId && f.FollowingId == follow.FollowingId);
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

