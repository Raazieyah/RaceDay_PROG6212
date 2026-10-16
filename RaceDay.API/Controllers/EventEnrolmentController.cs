[using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Contracts;
using RaceDay.Models;

namespace RaceDay.API.Controllers;//show post,get,get,put

[ApiController]
[Route("api/enrolments")]
public class EnrolmentController : ControllerBase
{
    private readonly RaceDayDBContext _context; 

    public EnrolmentController(RaceDayDBContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        return int.TryParse(userIdStr, out var id) ? id : null;
    }

    [HttpPost]//POST
    public async Task<IActionResult> Enroll([FromBody] CreateEnrolmentRequest dto) // 2️⃣ FIXED: Contract Name
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Participant" || currentUserId == null) return Forbid();

        //validation eventy
        var targetEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == dto.EventId); // 3️⃣ FIXED: Primary Key name
        if (targetEvent == null) return NotFound("Target event not found.");

        
        var alreadyEnrolled = await _context.Enrolments
            .AnyAsync(e => e.EventId == dto.EventId && e.ParticipantId == currentUserId.Value);
        if (alreadyEnrolled) return BadRequest("You are already enrolled in this event.");

        var enrolment = new Enrolment
        {
            ParticipantId = currentUserId.Value,
            EventId = dto.EventId,
            EnrolledAt = DateTime.UtcNow
            
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Enrolled successfully.", enrolmentId = enrolment.EnrolmentId });
    }

    // get
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Participant" || currentUserId == null) return Forbid();

        var myRecords = await _context.Enrolments
            .Where(e => e.ParticipantId == currentUserId.Value)
            .Include(e => e.Event)
                .ThenInclude(ev => ev.Category)
            .ToListAsync();

        return Ok(myRecords);
    }

    // get
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetEventEnrolments(int eventId)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        var targetEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
        if (targetEvent == null) return NotFound("Event not found.");
        if (targetEvent.OrganiserId != currentUserId.Value) return Forbid();

        var lists = await _context.Enrolments
            .Where(e => e.EventId == eventId)
            .Include(e => e.Event)
                .ThenInclude(ev => ev.Category)
            .Select(e => new
            {
                Id = e.EnrolmentId,
                e.ParticipantId,
                CategoryName = e.Event.Category.CategoryType
            })
            .ToListAsync();

        return Ok(lists);
    }

    // put
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateEnrolmentStatus(int id, [FromBody] UpdateEnrolmentStatusRequest dto)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        if (dto.Status != "Pending" && dto.Status != "Confirmed" && dto.Status != "Cancelled")
        {
            return BadRequest("Status must be 'Pending', 'Confirmed', or 'Cancelled'.");
        }

        var enrolment = await _context.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == id);

        if (enrolment == null) return NotFound("Enrolment record not found.");

        
        if (enrolment.Event.OrganiserId != currentUserId.Value) return Forbid();
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Enrolment status updated to '{dto.Status}' successfully." });
    }
}

