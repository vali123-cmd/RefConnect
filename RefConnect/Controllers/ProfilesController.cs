using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.EntityFrameworkCore;
using RefConnect.Models;
using RefConnect.DTOs.Users;
using RefConnect.Services.Interfaces;
using RefConnect.Services.Implementations;
using System.Security.Claims;





namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProfileService _profileService;

        public ProfilesController(UserManager<ApplicationUser> userManager, ProfileService profileService)
        {
            _userManager = userManager;
            _profileService = profileService;
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