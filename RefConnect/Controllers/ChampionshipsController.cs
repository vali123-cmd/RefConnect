using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.Championships;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    
    public class ChampionshipsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChampionshipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Championships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChampionshipDto>>> GetChampionships()
        {
            var championships = await _context.Championships
                .Select(c => new ChampionshipDto
                {
                    ChampionshipId = c.ChampionshipId,
                    Name = c.Name,
                    Season = c.Season
                })
                .ToListAsync();

            return Ok(championships);
        }

        // GET: api/Championships/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ChampionshipDto>> GetChampionship(string id)
        {
            var championship = await _context.Championships.FindAsync(id);

            if (championship == null)
            {
                return NotFound();
            }

            var championshipDto = new ChampionshipDto
            {
                ChampionshipId = championship.ChampionshipId,
                Name = championship.Name,
                Season = championship.Season
            };

            return Ok(championshipDto);
        }

        // POST: api/Championships
        [HttpPost]
        public async Task<ActionResult<ChampionshipDto>> CreateChampionship(CreateChampionshipDto createDto)
        {
            var championship = new Championship
            {
                ChampionshipId = Guid.NewGuid().ToString(),
                Name = createDto.Name,
                Season = createDto.Season
            };

            _context.Championships.Add(championship);
            await _context.SaveChangesAsync();

            var championshipDto = new ChampionshipDto
            {
                ChampionshipId = championship.ChampionshipId,
                Name = championship.Name,
                Season = championship.Season
            };

            return CreatedAtAction(nameof(GetChampionship), new { id = championship.ChampionshipId }, championshipDto);
        }

        // PUT: api/Championships/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChampionship(string id, UpdateChampionshipDto updateDto)
        {
            var championship = await _context.Championships.FindAsync(id);

            if (championship == null)
            {
                return NotFound();
            }

            championship.Name = updateDto.Name;
            championship.Season = updateDto.Season;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Championships/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChampionship(string id)
        {
            var championship = await _context.Championships.FindAsync(id);

            if (championship == null)
            {
                return NotFound();
            }

            _context.Championships.Remove(championship);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

