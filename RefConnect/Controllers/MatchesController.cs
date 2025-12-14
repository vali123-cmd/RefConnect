using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.DTOs.Matches;
using MatchModel = RefConnect.Models.Match;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches()
        {
            var matches = await _context.Matches
                .Select(m => new MatchDto
                {
                    MatchId = m.MatchId,
                    MatchDateTime = m.MatchDateTime,
                    Location = m.Location,
                    Score = m.Score,
                    Status = m.Status,
                    ChampionshipId = m.ChampionshipId
                })
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/Matches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(string id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            var matchDto = new MatchDto
            {
                MatchId = match.MatchId,
                MatchDateTime = match.MatchDateTime,
                Location = match.Location,
                Score = match.Score,
                Status = match.Status,
                ChampionshipId = match.ChampionshipId
            };

            return Ok(matchDto);
        }

        // POST: api/Matches
        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchDto createDto)
        {
            var match = new MatchModel
            {
                MatchId = Guid.NewGuid().ToString(),
                MatchDateTime = createDto.MatchDateTime,
                Location = createDto.Location,
                Score = createDto.Score,
                Status = createDto.Status,
                ChampionshipId = createDto.ChampionshipId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var matchDto = new MatchDto
            {
                MatchId = match.MatchId,
                MatchDateTime = match.MatchDateTime,
                Location = match.Location,
                Score = match.Score,
                Status = match.Status,
                ChampionshipId = match.ChampionshipId
            };

            return CreatedAtAction(nameof(GetMatch), new { id = match.MatchId }, matchDto);
        }

        // PUT: api/Matches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(string id, UpdateMatchDto updateDto)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            match.MatchDateTime = updateDto.MatchDateTime;
            match.Location = updateDto.Location;
            match.Score = updateDto.Score;
            match.Status = updateDto.Status;
            match.ChampionshipId = updateDto.ChampionshipId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Matches/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

