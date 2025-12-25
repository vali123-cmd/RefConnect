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
                        .Where(p => followedUserIds.Contains(p.UserId) || _context.Users
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

            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isFollowing = false;

            // daca requester nu e logat, verificam doar daca postarea apartine unui user cu profil public
            var postOwnerId = await _context.Posts
                .Where(p => p.PostId == id)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postOwnerId == null)
            {
                return NotFound();
            }
            var hasPublicProfile = await _context.Users
                .OfType<ApplicationUser>()
                .Where(u => u.Id == postOwnerId)
                .Select(u => u.IsProfilePublic)
                .FirstOrDefaultAsync(); 
            
            if(requesterId == null)
            {
                if (!hasPublicProfile)
                {
                    return Forbid();
                }
            }
            else
            {
                if (!isAdmin)
                {
                    if (requesterId != postOwnerId)
                    {
                        isFollowing = await _context.Follows
                            .AnyAsync(f => f.FollowerId == requesterId && f.FollowingId == postOwnerId);
                        if (!isFollowing && !hasPublicProfile)
                        {
                            return Forbid();
                        }
                    }
                }

            }

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
        //doar un user logat poate crea o postare
        [Authorize]
        [HttpPost]

        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createDto)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");   
            if (requesterId != createDto.UserId && !isAdmin)
            {
                return Forbid();
                // un user nu poate crea o postare in numele altui user
            }

            

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
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, UpdatePostDto updateDto)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var postOwnerId = await _context.Posts
                .Where(p => p.PostId == id)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postOwnerId == null)
            {
                return NotFound();
            }
            if (requesterId != postOwnerId && !isAdmin)
            {
                return Forbid();
            }

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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var postOwnerId = await _context.Posts
                .Where(p => p.PostId == id)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
            if (postOwnerId == null)
            {
                return NotFound();
            }
            if (requesterId != postOwnerId && !isAdmin)
            {
                return Forbid();
            }
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

