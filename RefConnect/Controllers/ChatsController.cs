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
            var chats = await _chatService.GetChatsForUserAsync(userId);
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
        
        
       
    }
}

