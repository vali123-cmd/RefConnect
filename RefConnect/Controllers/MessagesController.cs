using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Messages;

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
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createDto)
        {
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
        public async Task<IActionResult> UpdateMessage(string id, UpdateMessageDto updateDto)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            message.Content = updateDto.Content;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Messages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

