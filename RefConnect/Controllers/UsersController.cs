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

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}