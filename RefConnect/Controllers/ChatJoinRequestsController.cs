using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefConnect.DTOs.ChatJoinRequest;
using RefConnect.Services.Interfaces;
using System.Security.Claims;

namespace RefConnect.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatJoinRequestsController : ControllerBase
{
    private readonly IChatJoinRequestService _chatJoinRequestService;

    public ChatJoinRequestsController(IChatJoinRequestService chatJoinRequestService)
    {
        _chatJoinRequestService = chatJoinRequestService;
    }

   
    [HttpGet("owner")]
    public async Task<ActionResult<IEnumerable<ChatJoinRequestDto>>> GetRequestsForOwner()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var requests = await _chatJoinRequestService.GetPendingRequestsForOwnerAsync(userId);
        return Ok(requests);
    }

   
    [HttpGet("chat/{chatId}")]
    public async Task<ActionResult<IEnumerable<ChatJoinRequestDto>>> GetRequestsForChat(string chatId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var requests = await _chatJoinRequestService.GetPendingRequestsForChatAsync(chatId, userId);
        return Ok(requests);
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<ChatJoinRequestDto>>> GetMyPendingRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var requests = await _chatJoinRequestService.GetUserPendingRequestsAsync(userId);
        return Ok(requests);
    }

   
    [HttpPost]
    public async Task<ActionResult<ChatJoinRequestDto>> CreateJoinRequest([FromBody] CreateChatJoinRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var request = await _chatJoinRequestService.CreateJoinRequestAsync(userId, dto.ChatId);
        if (request == null)
        {
            return BadRequest("Unable to create join request. Chat may not exist, you may already be a member, or a pending request already exists.");
        }

        return CreatedAtAction(nameof(GetMyPendingRequests), request);
    }

    
    [HttpPost("{requestId}/accept")]
    public async Task<IActionResult> AcceptRequest(string requestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var success = await _chatJoinRequestService.AcceptRequestAsync(requestId, userId);
        if (!success)
        {
            return NotFound("Request not found or you don't have permission to accept it.");
        }

        return Ok(new { message = "Request accepted successfully." });
    }

   
    [HttpPost("{requestId}/decline")]
    public async Task<IActionResult> DeclineRequest(string requestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var success = await _chatJoinRequestService.DeclineRequestAsync(requestId, userId);
        if (!success)
        {
            return NotFound("Request not found or you don't have permission to decline it.");
        }

        return Ok(new { message = "Request declined successfully." });
    }

    
    [HttpDelete("{requestId}")]
    public async Task<IActionResult> CancelRequest(string requestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var success = await _chatJoinRequestService.CancelRequestAsync(requestId, userId);
        if (!success)
        {
            return NotFound("Request not found or already processed.");
        }

        return NoContent();
    }
}
