using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Comments;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RefConnect.Services.Interfaces;
using RefConnect.Services.Implementations;
using RefConnect.DTOs.FollowRequest;


namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFollowRequestService _followRequestService;
    
        public FollowRequestsController(ApplicationDbContext context, IFollowRequestService followRequestService)
        {
            _context = context;
            _followRequestService = followRequestService;
        }

        // POST: api/FollowRequests
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendFollowRequest([FromBody] FollowRequestDto followRequestDto)
        {
            var followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followingId = followRequestDto.FollowingId;
            var result = await _followRequestService.SendFollowRequestAsync(followerId, followingId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Follow request could not be sent.");
        }

        //cancel
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> CancelFollowRequest([FromBody] FollowRequestDto followRequestDto)
        {
            var followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followingId = followRequestDto.FollowingId;
            var result = await _followRequestService.CancelFollowRequestAsync(followerId, followingId);
            if (result)
            {
                return Ok();

            }   
            return BadRequest("Follow request could not be canceled.");
        }

        // GET: api/FollowRequests/Pending
        [HttpGet("Pending")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FollowRequest>>> GetPendingFollowRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _followRequestService.GetPendingFollowRequestsAsync(userId);
            return Ok(requests);
        }

        // POST: api/FollowRequests/Accept
        [HttpPost("Accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFollowRequest([FromBody] FollowRequestDto followRequestDto)
        {
            var followingId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followerId = followRequestDto.FollowerId;
            var result = await _followRequestService.AcceptFollowRequestAsync(followingId, followerId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Follow request could not be accepted.");

        }
        // POST: api/FollowRequests/Decline
        [HttpPost("Decline")]
        [Authorize]
        public async Task<IActionResult> DeclineFollowRequest([FromBody] FollowRequestDto followRequestDto)
        {
            var followingId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followerId = followRequestDto.FollowerId;
            var result = await _followRequestService.DeclineFollowRequestAsync(followingId, followerId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Follow request could not be declined.");
        }

    }
}
