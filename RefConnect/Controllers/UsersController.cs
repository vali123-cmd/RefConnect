using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization; // Needed to lock endpoints
using Microsoft.EntityFrameworkCore;
using RefConnect.Models;
using RefConnect.DTOs.Users;
using System.Security.Claims;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        public UsersController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

     
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new UserDto 
                {
                    Id = u.Id,

                    Email = u.Email,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Description = u.Description,
                    ProfileImageUrl = u.ProfileImageUrl,
                    IsProfilePublic = u.IsProfilePublic,
                    CreatedAt = u.CreatedAt,
                    FollowersCount = u.FollowersCount,
                    FollowingCount = u.FollowingCount,

                })
                .ToListAsync();

            return Ok(users);
        }
        //GET: api/Users/{id}/profile-image
        [HttpGet("{id}/profile-image")]
        public async Task<ActionResult<string>> GetProfileImageUrl(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new { profileImageUrl = user.ProfileImageUrl });
        }
        // POST: api/Users/{id}/profile-image

        [HttpPost("{id}/profile-image")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage(string id, [FromForm] IFormFile file)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (requesterId != id && !isAdmin) return Forbid();

            if (file == null || file.Length == 0) return BadRequest(new { error = "file is required." });

            // validate content type and size
            var allowed = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowed.Contains(file.ContentType?.ToLowerInvariant()))
                return BadRequest(new { error = "invalid file type." });
            const long maxBytes = 10 * 1024 * 1024; // 10 MB
            if (file.Length > maxBytes) return BadRequest(new { error = "file too large (max 10MB)." });

            // determine uploads folder, fallback if WebRootPath is null
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(_environment.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsFolder = Path.Combine(webRoot, "uploads", "profile-images");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(file.FileName);
            var safeFileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // public url (served from wwwroot)
            var publicUrl = $"/uploads/profile-images/{safeFileName}";

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.ProfileImageUrl = publicUrl;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);

            return Ok(new { url = publicUrl });
        }
    
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            //only owner or admin can see full user details
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (requesterId != id && !isAdmin)
            {
                return Forbid();
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                ProfileImageUrl = user.ProfileImageUrl,
                Description = user.Description,
                IsProfilePublic = user.IsProfilePublic,
                FollowersCount = user.FollowersCount,
                FollowingCount = user.FollowingCount

            };
        }

        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto model)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (requesterId != id && !isAdmin)
            {
                return Forbid();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Map incoming DTO changes to the entity
            user.FirstName = model.FirstName;
            user.UserName = model.UserName;
            user.LastName = model.LastName;
            user.Description = model.Description;
            user.ProfileImageUrl = model.ProfileImageUrl;
            user.IsProfilePublic = model.IsProfilePublic;
            
            

      
            

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }


       
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (requesterId != id && !isAdmin)
            {
                return Forbid();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Soft delete: keep the user row to avoid FK constraint failures.
            // Disable login and anonymize personal data.
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            user.EmailConfirmed = false;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;

            // Anonymize fields used in the app
            user.UserName = $"deleted_{user.Id}";
            user.NormalizedUserName = user.UserName.ToUpperInvariant();
            // Use a dummy email to satisfy the [Required] validation and UniqueEmail constraint
            user.Email = $"deleted_{user.Id}@refconnect.local";
            user.NormalizedEmail = user.Email.ToUpperInvariant();
            user.PhoneNumber = null;

            user.FirstName = "Deleted";
            user.LastName = "User";
            user.Description = "[deleted]";
            user.ProfileImageUrl = null;
            user.IsProfilePublic = false;

            // Replace password with a random one so existing credentials no longer work.
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var randomPassword = $"{Guid.NewGuid():N}aA1!";
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, randomPassword);
            if (!resetResult.Succeeded)
            {
                return BadRequest(resetResult.Errors);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}