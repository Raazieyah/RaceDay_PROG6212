using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.Contracts;

namespace RaceDay.API.Controllers;

//step13 + 14 qand 15 16
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDBContext _context;

    public AuthController(RaceDayDBContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = PasswordHasher.HashPassword(dto.Password),
            RoleId = calculatedRoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("Registration successful.");
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        // Step 14: Save identity details inside 
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("Role", user.Role.Name);

        return Ok(new AuthResponse(
            Token: "session-managed", // Using session instead of JWT
            Id: user.Id.ToString(),
            Email: user.Email,
            Role: user.Role.Name
        ));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok("Logged out successfully.");
    }
}