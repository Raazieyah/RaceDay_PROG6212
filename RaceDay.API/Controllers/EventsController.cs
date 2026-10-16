using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Contracts;
using Events.Models;
using RaceDay.Models;

namespace RaceDay.API.Controllers; //getx3,post,put,delete

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDBContext _context;

    public EventsController(RaceDayDBContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        return int.TryParse(userIdStr, out var id) ? id : null;
    }

    // GE(1)
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _context.Events.ToListAsync();
        return Ok(events);
    }

    // GET(2)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var raceEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (raceEvent == null) return NotFound("Event not found.");
        return Ok(raceEvent);
    }

    // GET(3)
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyEvents()
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        var myEvents = await _context.Events
            .Where(e => e.OrganiserId == currentUserId.Value)
            .ToListAsync();

        return Ok(myEvents);
    }

    // post
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] UpdateEventRequest dto)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        var newEvent = new Event
        {
            EventName = dto.Name,
            Description = dto.Description,
            EventDate = dto.Date,
            Location = dto.Location,
            OrganiserId = currentUserId.Value,
            CategoryId = 1 // Default category ID for testing, or expand your DTO to receive this
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventId }, newEvent);
    }

    // put
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest dto)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (existingEvent == null) return NotFound("Event not found.");

        if (existingEvent.OrganiserId != currentUserId.Value) return Forbid();

        existingEvent.EventName = dto.Name;
        existingEvent.Description = dto.Description;
        existingEvent.EventDate = dto.Date;
        existingEvent.Location = dto.Location;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Event updated successfully." });
    }

    // delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
        if (existingEvent == null) return NotFound("Event not found.");

        if (existingEvent.OrganiserId != currentUserId.Value) return Forbid();

        _context.Events.Remove(existingEvent);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Event deleted successfully." });
    }
}


[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDBContext _context;

    public EnrolmentsController(RaceDayDBContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CompleteEnrolment([FromBody] CreateEnrolmentRequest dto)
    {
        var role = HttpContext.Session.GetString("Role");
        var userIdStr = HttpContext.Session.GetString("UserId");

        if (role != "Participant" || !int.TryParse(userIdStr, out int participantId))
            return Forbid();

        // checking if event exists
        var eventExists = await _context.Events.AnyAsync(e => e.EventId == dto.EventId);
        if (!eventExists) return NotFound("Target event not found.");

        // Check for duplication
        var alreadyEnrolled = await _context.Enrolments
            .AnyAsync(e => e.EventId == dto.EventId && e.ParticipantId == participantId);

        if (alreadyEnrolled) return BadRequest("You are already enrolled in this event.");

        var enrolment = new Enrolment
        {
            EventId = dto.EventId,
            ParticipantId = participantId,
            EnrolledAt = DateTime.UtcNow
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Enrolment processed successfully for Participant." });
    }
}
