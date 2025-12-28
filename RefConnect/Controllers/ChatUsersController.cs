using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.ChatUsers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChatUsers
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatUserDto>>> GetChatUsers()
        {
            var chatUsers = await _context.ChatUsers
                .Select(cu => new ChatUserDto
                {
                    ChatUserId = cu.ChatUserId,
                    ChatId = cu.ChatId,
                    UserId = cu.UserId
                })
                .ToListAsync();

            return Ok(chatUsers);
        }

        // GET: api/ChatUsers/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]

        public async Task<ActionResult<ChatUserDto>> GetChatUser(string id)
        {
            var chatUser = await _context.ChatUsers.FindAsync(id);

            if (chatUser == null)
            {
                return NotFound();
            }

            var chatUserDto = new ChatUserDto
            {
                ChatUserId = chatUser.ChatUserId,
                ChatId = chatUser.ChatId,
                UserId = chatUser.UserId
            };

            return Ok(chatUserDto);
        }

        // POST: api/ChatUsers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ChatUserDto>> CreateChatUser(CreateChatUserDto createDto)
        {
            var chatUser = new ChatUser
            {
                ChatUserId = Guid.NewGuid().ToString(),
                ChatId = createDto.ChatId,
                UserId = createDto.UserId
            };

            _context.ChatUsers.Add(chatUser);
            await _context.SaveChangesAsync();

            var chatUserDto = new ChatUserDto
            {
                ChatUserId = chatUser.ChatUserId,
                ChatId = chatUser.ChatId,
                UserId = chatUser.UserId
            };

            return CreatedAtAction(nameof(GetChatUser), new { id = chatUser.ChatUserId }, chatUserDto);
        }

        // PUT: api/ChatUsers/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChatUser(string id, UpdateChatUserDto updateDto)
        {
            var chatUser = await _context.ChatUsers.FindAsync(id);

            if (chatUser == null)
            {
                return NotFound();
            }

            chatUser.ChatId = updateDto.ChatId;
            chatUser.UserId = updateDto.UserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ChatUsers/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatUser(string id)
        {
            var chatUser = await _context.ChatUsers.FindAsync(id);

            if (chatUser == null)
            {
                return NotFound();
            }

            _context.ChatUsers.Remove(chatUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

