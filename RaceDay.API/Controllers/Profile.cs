using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize] 
public class ProfileController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET API for profile
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized("User is not logged in.");

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound("Profile not found.");

        
        return Ok(new ProfileDto(
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Role: user.Role?.Name ?? "Participant"
        ));
    }

    // PUT profile
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (!int.TryParse(userIdString, out int userId))
            return Unauthorized("User is not logged in.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound("User profile not found.");

        // Updated structural properties
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Profile updated successfully." });
    }
}
