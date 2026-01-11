using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RefConnect.Models;
using RefConnect.DTOs.Users;

namespace RefConnect.Controllers;



[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.UserName,
            Description = model.Description ?? string.Empty,
            ProfileImageUrl = model.ProfileImageUrl ?? string.Empty
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        await _userManager.AddToRoleAsync(user, "User");

        // Generate JWT token after registration
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            Message = "User registered successfully",
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        return Unauthorized(new { Message = "Email sau parola incorecta." });

        var userRoles = await _userManager.GetRolesAsync(user);

        
    var authClaims = new List<Claim>
    {

        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),

        
        new Claim(JwtRegisteredClaimNames.Jti,
        Guid.NewGuid().ToString())
    };
    authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        expires: DateTime.Now.AddHours(1),
        claims: authClaims,
        signingCredentials: new SigningCredentials(
        new
        SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[
        "Jwt:Key"])),
        SecurityAlgorithms.HmacSha256)
        );
        return Ok(new { Token = new
        JwtSecurityTokenHandler().WriteToken(token) 
    });
    }
}