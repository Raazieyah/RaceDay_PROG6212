using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Controllers;//show post,get,get

[ApiController]
[Route("api/results")]
[Authorize]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDBContext _context;

    public ResultsController(RaceDayDBContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        return int.TryParse(userIdStr, out var id) ? id : null;
    }

    // post
    [HttpPost]
    public async Task<IActionResult> CreateResult([FromBody] CreateResultsRequest dto) 
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Organiser" || currentUserId == null) return Forbid();

        
        if (!int.TryParse(dto.UserId, out int participantId))
            return BadRequest("Invalid Participant User ID format.");

        var enrolment = await _context.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EventId == dto.EventId && e.ParticipantId == participantId);

        if (enrolment == null)
            return NotFound("No enrolment record found for this participant at this event.");

  
        if (enrolment.Event.OrganiserId != currentUserId.Value)
            return Forbid();

        var resultExists = await _context.Results
            .AnyAsync(r => r.EventId == dto.EventId && r.ParticipantId == participantId);

        if (resultExists)
            return BadRequest("A result has already been recorded for this participant at this event.");

        int.TryParse(dto.Position, out int positionInt);

        var newResult = new Result
        {
            EventId = dto.EventId,
            ParticipantId = participantId,
            FinishTime = dto.TimeTaken, 
            FinishPosition = positionInt 
        };

        _context.Results.Add(newResult);
        await _context.SaveChangesAsync();

        return StatusCode(201, newResult);
    }

    //get
    [HttpGet("mine")]
    public async Task<IActionResult> GetMyResults()
    {
        var role = HttpContext.Session.GetString("Role");
        var currentUserId = GetCurrentUserId();

        if (role != "Participant" || currentUserId == null) return Forbid();

        var personalResults = await _context.Results
            .Include(r => r.Event)
                .ThenInclude(e => e.Category)
            .Where(r => r.ParticipantId == currentUserId.Value)
            .Select(r => new
            {
                Id = r.ResultId, 
                EventName = r.Event.EventName, 
                CategoryName = r.Event.Category.CategoryType,
                FinishTime = r.FinishTime,
                Position = r.FinishPosition
            })
            .ToListAsync();

        return Ok(personalResults);
    }

    // get
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetEventResults(int eventId)
    {
        
        var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId); 
        if (!eventExists)
            return NotFound("Target event not found.");

        var eventResults = await _context.Results
            .Include(r => r.Event)
                .ThenInclude(e => e.Category)
            .Where(r => r.EventId == eventId)
            .Select(r => new
            {
                Id = r.ResultId,
                r.ParticipantId,
                CategoryName = r.Event.Category.CategoryType,
                r.FinishTime,
                Position = r.FinishPosition
            })
            .ToListAsync();

        return Ok(eventResults);
    }
}

