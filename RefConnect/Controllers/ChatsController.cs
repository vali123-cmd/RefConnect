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



        // GET: api/Chats
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chats = await _chatService.GetChatsForUserAsync(userId);
            return Ok(chats);
        }

        [Authorize]
        // POST: api/Chats/direct
        [HttpPost("direct")]
        public async Task<ActionResult<ChatDto>> CreateDirectChat([FromBody] CreateChatDto dto)

        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.CreateDirectChatAsync(userId, dto.UserId);
            return Ok(chat);
        }

        
        

        
        
        
       
    }
}

