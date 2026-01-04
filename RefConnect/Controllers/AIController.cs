using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.EntityFrameworkCore;
using RefConnect.Models;
using RefConnect.DTOs.Users;
using RefConnect.Services.Interfaces;
using RefConnect.Services.Implementations;
using System.Security.Claims;


namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AIController : ControllerBase
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefinePostTextAI _aiService;

        public AIController(UserManager<ApplicationUser> userManager, IRefinePostTextAI aiService)
        {
            _userManager = userManager;
            _aiService = aiService;
        }

        // api/ai/refine-post-text
        [HttpPost("refine-post-text")]
        [Authorize]
        public async Task<ActionResult<string>> RefinePostText([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest(new { error = "text body is required." });

            var refinedText = await _aiService.RefineTextAsync(text);
            return Ok(refinedText);
        }
        [HttpPost("appropriate-content")]
        
        public async Task<ActionResult<bool>> IsContentAppropriate([FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest(new { error = "content query parameter is required." });

            var isAppropriate = await _aiService.IsContentAppropriateAsync(content);
            return Ok(isAppropriate);
            
        }
    }
}
