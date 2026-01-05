using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Chats;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RefConnect.Services.Implementations;
using RefConnect.Services.Interfaces;




namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IChatService _chatService;

        public ChatsController(ApplicationDbContext context, IChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }




        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string chatType = "group";
            var chats = await _chatService.GetAllChatsAsync(chatType);
            return Ok(chats);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> SearchChats([FromQuery] string query, [FromQuery] string? chatType = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var chats = await _chatService.SearchChatsAsync(userId, query, isAdmin, chatType);
            return Ok(chats);
        }
    

        

        [Authorize]
        [HttpPost("group")]
        public async Task<ActionResult<ChatDto>> CreateGroupChat([FromBody] CreateGroupChatDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.CreateGroupChatAsync(userId, dto);
            return Ok(chat);
        }

        [Authorize]
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(string chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var result = await _chatService.DeleteChatAsync(userId, chatId, isAdmin);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPut("{chatId}")]
        public async Task<IActionResult> UpdateChat(string chatId, [FromBody] UpdateChatDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var result = await _chatService.UpdateChatAsync(userId, chatId, dto, isAdmin);
            if (result)
            {
                return NoContent();
            }
            return NotFound();

        }

       
        [Authorize]
        [HttpGet("{chatId}/is-member/{userId}")]
        public async Task<ActionResult<bool>> IsUserMemberOfGroupChat(string chatId, string userId)
        {
            
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && requesterId != userId)
            {
                return Forbid();
            }

       
            var chat = await _context.Chats
                .AsNoTracking()
                .Where(c => c.ChatId == chatId)
                .Select(c => new { c.ChatId, c.ChatType, c.Name })
                .FirstOrDefaultAsync();

            if (chat == null)
                return NotFound();

            if (chat.ChatType != "group")
                return BadRequest();

            
            var isMember = await _context.ChatUsers
                .AsNoTracking()
                .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
                .AnyAsync();

            return Ok(isMember);
        }


        [Authorize]
        [HttpDelete("{chatId}/members/{userId}")]
        public async Task<IActionResult> RemoveUserFromChat(string chatId, string userId)
        {
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(requesterId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");

            var chatInfo = await _context.Chats
                .AsNoTracking()
                .Where(c => c.ChatId == chatId)
                .Select(c => new { c.ChatId, c.ChatType, c.CreatedByUserId })
                .FirstOrDefaultAsync();

            if (chatInfo == null)
                return NotFound();

            if (chatInfo.ChatType != "group")
                return BadRequest("This endpoint is only for group chats.");

            var isSelfRemoval = requesterId == userId;

            if (!isSelfRemoval && !isAdmin && chatInfo.CreatedByUserId != requesterId)
                return Forbid();

            
            var ok = await _chatService.RemoveUserFromGroupAsync(chatId, requesterId, userId);
            if (!ok) return NotFound();

            return NoContent();
        }
        
        
       
    }
}

