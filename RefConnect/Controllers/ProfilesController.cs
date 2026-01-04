using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.EntityFrameworkCore;
using RefConnect.Models;
using RefConnect.DTOs.Users;
using RefConnect.Services.Interfaces;
using System.Security.Claims;





namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileService _profileService;

        public ProfilesController(UserManager<ApplicationUser> userManager, IProfileService profileService)
        {
            _userManager = userManager;
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
        {
            var users = await _userManager.Users
                .Select(u => new ProfileDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Description = u.Description,
                    ProfileImageUrl = u.ProfileImageUrl,
                    IsProfilePublic = u.IsProfilePublic,
                    FollowersCount = u.FollowersCount,
                    FollowingCount = u.FollowingCount,
                })
                .ToListAsync();

            return Ok(users);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> SearchUsers([FromQuery] string query)
        {
            
            //using ProfileService to search users
            var profiles = await _profileService.SearchUsersAsync(query);
            return Ok(profiles);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ProfileDto?>> GetProfile(string userId, [FromQuery] string? requesterId = null)
        {
            var profile = await _profileService.GetProfileAsync(userId, requesterId);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }
        [HttpGet("{userId}/extended")]
        public async Task<ActionResult<ProfileExtendedDto?>> GetProfileExtended(string userId)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var canView = await _profileService.mayViewProfileExtendedAsync(userId, requesterId);
            if (!canView) 
            {
                return Forbid();
            }
            var profileExtended = await _profileService.GetProfileExtendedAsync(userId, requesterId);
            if (profileExtended == null)
            {
                return NotFound();
            }
            return Ok(profileExtended);
        }

    
    }
}