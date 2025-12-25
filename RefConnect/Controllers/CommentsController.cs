using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Comments;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost(string postId)
        {
            //poti obtine comentariile doar de la postari la care ai acces
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == postId); 
            if (post == null)
            {
                return NotFound("Postarea nu exista");

            }
            var postOwner = post.User;
            //daca profilul e privat si nu esti ownerul postarii sau admin, nu ai acces
            var isFollower = await _context.Follows
                .AnyAsync(f => f.FollowerId == userId && f.FollowingId == postOwner.Id);
            
            if (!postOwner.IsProfilePublic && postOwner.Id != userId && !isAdmin && !isFollower)
            {
                return Forbid();
            }
            
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
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
        [Authorize(Roles = "Admin")]
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
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if(!isAdmin)
            {
                
                var commentToCheck = await _context.Comments
                    .Include(c => c.Post)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(c => c.CommentId == id);

                if (commentToCheck == null)
                {
                    return NotFound();
                }

                var postOwner = commentToCheck.Post.User;
                var isFollower = await _context.Follows
                    .AnyAsync(f => f.FollowerId == requesterId && f.FollowingId == postOwner.Id);
                if (!postOwner.IsProfilePublic && postOwner.Id != requesterId && !isAdmin && !isFollower)
                {
                    return Forbid();
                }
            }

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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createDto)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (createDto.UserId != requesterId && !isAdmin)
            {
                return Forbid(); //nu poti comenta in numele altui user
            }

         
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == createDto.PostId);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            var postOwner = post.User;
            var isFollower = await _context.Follows
                .AnyAsync(f => f.FollowerId == requesterId && f.FollowingId == postOwner.Id);
            if (!postOwner.IsProfilePublic && postOwner.Id != requesterId && !isAdmin && !isFollower)
            {
                return Forbid();
            }

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
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var commentToUpdate = await _context.Comments.FindAsync(id);
            if (commentToUpdate == null)
            {
                return NotFound("Comment not found");
            }
            if (commentToUpdate.UserId != requesterId && !isAdmin)
            {
                return Forbid("You are not authorized to update this comment");
            }
            var isFollower = await _context.Follows
                .AnyAsync(f => f.FollowerId == requesterId && f.FollowingId == commentToUpdate.Post.UserId);
            if (!commentToUpdate.Post.User.IsProfilePublic && commentToUpdate.Post.User.Id != requesterId && !isAdmin && !isFollower)
            {
                return Forbid("You are not authorized to update this comment");
            }


            

            commentToUpdate.Content = updateDto.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Comments/{id}
        [Authorize]
        [HttpDelete("{id}")]
        //doar useri logati si admini pot sterge comentarii

        public async Task<IActionResult> DeleteComment(string id)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            var comment = await _context.Comments.FindAsync(id);
            if(comment == null)
            {
                return NotFound("Comment not found");
            }
            if(requesterId != comment.UserId && !isAdmin)
            {
                return Forbid("You are not authorized to delete this comment");
            }

            

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

