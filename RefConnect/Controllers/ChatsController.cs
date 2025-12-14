using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Chats;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetChats()
        {
            var chats = await _context.Chats
                .Select(c => new ChatDto
                {
                    ChatId = c.ChatId,
                    ChatType = c.ChatType,
                    CreatedAt = c.CreatedAt,
                    ExpiresAt = c.ExpiresAt,
                    IsActive = c.IsActive,
                    MatchId = c.MatchId
                })
                .ToListAsync();

            return Ok(chats);
        }

        // GET: api/Chats/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDto>> GetChat(string id)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            var chatDto = new ChatDto
            {
                ChatId = chat.ChatId,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt,
                ExpiresAt = chat.ExpiresAt,
                IsActive = chat.IsActive,
                MatchId = chat.MatchId
            };

            return Ok(chatDto);
        }

        // POST: api/Chats
        [HttpPost]
        public async Task<ActionResult<ChatDto>> CreateChat(CreateChatDto createDto)
        {
            var chat = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                ChatType = createDto.ChatType,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = createDto.ExpiresAt,
                IsActive = createDto.IsActive,
                MatchId = createDto.MatchId
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            var chatDto = new ChatDto
            {
                ChatId = chat.ChatId,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt,
                ExpiresAt = chat.ExpiresAt,
                IsActive = chat.IsActive,
                MatchId = chat.MatchId
            };

            return CreatedAtAction(nameof(GetChat), new { id = chat.ChatId }, chatDto);
        }

        // PUT: api/Chats/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChat(string id, UpdateChatDto updateDto)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            chat.ChatType = updateDto.ChatType;
            chat.ExpiresAt = updateDto.ExpiresAt;
            chat.IsActive = updateDto.IsActive;
            chat.MatchId = updateDto.MatchId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Chats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(string id)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

