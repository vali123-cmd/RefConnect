using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Messages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Chat/{chatId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForChat(string chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdminApp = User.IsInRole("Admin");
            if (string.IsNullOrEmpty(userId) && !isAdminApp)
            {
                return Unauthorized();
            }
            //userul trebuie sa fie membru al chatului pentru a vedea mesajele
            //se verifica daca userId este membru al chatului cu chatId
            var isMember = await _context.ChatUsers
                .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);
            if (!isMember && !isAdminApp)
            {
                return Forbid();
            }
            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    ChatId = m.ChatId,
                    UserId = m.UserId
                })
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
            return Ok(messages);

        }
        
        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages()
        {
            var messages = await _context.Messages
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    ChatId = m.ChatId,
                    UserId = m.UserId
                })
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/Messages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetMessage(string id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            var messageDto = new MessageDto
            {
                MessageId = message.MessageId,
                Content = message.Content,
                SentAt = message.SentAt,
                ChatId = message.ChatId,
                UserId = message.UserId
            };

            return Ok(messageDto);
        }

        // POST: api/Messages
      
        [HttpPost]
          [Authorize]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            if(createDto.UserId != userId)
            {
                return Forbid();
            }
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Content = createDto.Content,
                SentAt = DateTime.UtcNow,
                ChatId = createDto.ChatId,
                UserId = createDto.UserId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = new MessageDto
            {
                MessageId = message.MessageId,
                Content = message.Content,
                SentAt = message.SentAt,
                ChatId = message.ChatId,
                UserId = message.UserId
            };

            return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId }, messageDto);
        }

        // PUT: api/Messages/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMessage(string id, UpdateMessageDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdminApp = User.IsInRole("Admin");
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            if (userId != message.UserId && !isAdminApp)
            {
                return Forbid();
            }

            message.Content = updateDto.Content;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Messages/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdminApp = User.IsInRole("Admin");
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            if (userId != message.UserId && !isAdminApp)
            {
                return Forbid();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

