using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefConnect.Data;
using RefConnect.Models;
using RefConnect.DTOs.MatchAssigments;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MatchAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MatchAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchAssignmentDto>>> GetMatchAssignments()
        {
            var assignments = await _context.MatchAssignments
                .Select(ma => new MatchAssignmentDto
                {
                    MatchAssignmentId = ma.MatchAssignmentId,
                    RoleInMatch = ma.RoleInMatch,
                    UserId = ma.UserId,
                    MatchId = ma.MatchId
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/MatchAssignments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchAssignmentDto>> GetMatchAssignment(string id)
        {
            var assignment = await _context.MatchAssignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            var assignmentDto = new MatchAssignmentDto
            {
                MatchAssignmentId = assignment.MatchAssignmentId,
                RoleInMatch = assignment.RoleInMatch,
                UserId = assignment.UserId,
                MatchId = assignment.MatchId
            };

            return Ok(assignmentDto);
        }

        // POST: api/MatchAssignments
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MatchAssignmentDto>> CreateMatchAssignment(CreateMatchAssignmentDto createDto)
        {
            var assignment = new MatchAssignment
            {
                MatchAssignmentId = Guid.NewGuid().ToString(),
                RoleInMatch = createDto.RoleInMatch,
                UserId = createDto.UserId,
                MatchId = createDto.MatchId
            };

            _context.MatchAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            var assignmentDto = new MatchAssignmentDto
            {
                MatchAssignmentId = assignment.MatchAssignmentId,
                RoleInMatch = assignment.RoleInMatch,
                UserId = assignment.UserId,
                MatchId = assignment.MatchId
            };

            return CreatedAtAction(nameof(GetMatchAssignment), new { id = assignment.MatchAssignmentId }, assignmentDto);
        }

        // PUT: api/MatchAssignments/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatchAssignment(string id, UpdateMatchAssignmentDto updateDto)
        {
            var assignment = await _context.MatchAssignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.RoleInMatch = updateDto.RoleInMatch;
            assignment.UserId = updateDto.UserId;
            assignment.MatchId = updateDto.MatchId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MatchAssignments/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatchAssignment(string id)
        {
            var assignment = await _context.MatchAssignments.FindAsync(id);


            if (assignment == null)
            {

                return NotFound();
            }

            _context.MatchAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

