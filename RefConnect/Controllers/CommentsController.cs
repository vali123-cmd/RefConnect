using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Comments;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var comments = await _context.Comments
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    ParentCommentId = c.ParentCommentId
                })
                .ToListAsync();

            return Ok(comments);
        }

        // GET: api/Comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(string id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            var commentDto = new CommentDto
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentCommentId = comment.ParentCommentId
            };

            return Ok(commentDto);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createDto)
        {
            var comment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow,
                PostId = createDto.PostId,
                UserId = createDto.UserId,
                ParentCommentId = createDto.ParentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var commentDto = new CommentDto
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentCommentId = comment.ParentCommentId
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.CommentId }, commentDto);
        }

        // PUT: api/Comments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(string id, UpdateCommentDto updateDto)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            comment.Content = updateDto.Content;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

