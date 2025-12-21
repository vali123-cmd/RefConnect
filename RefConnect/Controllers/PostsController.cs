using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Posts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// nu includ comentariile in postari, pentru a face rost de comentariile unei postari, se va face o alta cerere API.
namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {

            // we should check which posts the requester is allowed to see here: posts from followed users)
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            //if user is not logged in, we return posts where the user has public profile
            //if user is admin, we return all posts

           
            if (requesterId != null)
            {
                if (!isAdmin)
                {
                    var followedUserIds = await _context.Users
                        .OfType<ApplicationUser>()
                        .Where(u => _context.Follows
                            .Any(f => f.FollowerId == requesterId && f.FollowingId == u.Id))
                        .Select(u => u.Id)
                        .ToListAsync();

                    var posts = await _context.Posts
                        .Where(p => followedUserIds.Contains(p.UserId))
                        .Select(p => new PostDto
                        {
                            PostId = p.PostId,
                            MediaType = p.MediaType,
                            MediaUrl = p.MediaUrl,
                            Description = p.Description,
                            CreatedAt = p.CreatedAt,
                            UserId = p.UserId
                        })
                        .ToListAsync();

                    return Ok(posts);
                }
                else
                {
                    var posts = await _context.Posts
                        .Select(p => new PostDto
                        {
                            PostId = p.PostId,
                            MediaType = p.MediaType,
                            MediaUrl = p.MediaUrl,
                            Description = p.Description,
                            CreatedAt = p.CreatedAt,
                            UserId = p.UserId
                        })
                        .ToListAsync();

                    return Ok(posts);
                } //returnez toate postarile pentru admin
            }
            else
            {
                var posts = await _context.Posts
                    .Where(p => _context.Users
                        .OfType<ApplicationUser>()
                        .Where(u => u.IsProfilePublic)
                        .Select(u => u.Id)
                        .Contains(p.UserId))
                    .Select(p => new PostDto
                    {
                        PostId = p.PostId,
                        MediaType = p.MediaType,
                        MediaUrl = p.MediaUrl,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        UserId = p.UserId
                    })
                    .ToListAsync();

                return Ok(posts);
            }
        }

        // GET: api/Posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(string id)
        {
            //verific daca un user poate obtine o anumita postare

            var foll
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var postDto = new PostDto
            {
                PostId = post.PostId,
                MediaType = post.MediaType,
                MediaUrl = post.MediaUrl,
                Description = post.Description,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId
            };

            return Ok(postDto);
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createDto)
        {
            var post = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                MediaType = createDto.MediaType,
                MediaUrl = createDto.MediaUrl,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = createDto.UserId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var postDto = new PostDto
            {
                PostId = post.PostId,
                MediaType = post.MediaType,
                MediaUrl = post.MediaUrl,
                Description = post.Description,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId
            };

            return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, postDto);
        }

        // PUT: api/Posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, UpdatePostDto updateDto)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.MediaType = updateDto.MediaType;
            post.MediaUrl = updateDto.MediaUrl;
            post.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

