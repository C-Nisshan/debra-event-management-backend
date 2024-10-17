using DebraSheru.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly EventManagementContext _context;

    public EventController(EventManagementContext context)
    {
        _context = context;
    }

    // GET: api/Event
    [HttpGet("GetEvents")]
    [Authorize(Roles = "Admin, Partner")]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.Events.ToListAsync();
    }

    // GET: api/Event/{id}
    [HttpGet("GetEventById/{id}")]
    [Authorize(Roles = "Admin, Partner")]
    public async Task<ActionResult<Event>> GetEvent(int id)
    {
        var @event = await _context.Events.FindAsync(id);

        if (@event == null)
        {
            return NotFound();
        }

        return Ok(@event);
    }

    // POST: api/Event
    [HttpPost("CreateEvent")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Event>> CreateEvent(Event @event)
    {
        _context.Events.Add(@event);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = @event.EventID }, @event);
    }

    // PUT: api/Event/{id}
    [HttpPut("UpdateEventById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEvent(int id, Event @event)
    {
        if (id != @event.EventID)
        {
            return BadRequest();
        }

        _context.Entry(@event).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Event/{id}
    [HttpDelete("DeleteEventById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
        {
            return NotFound();
        }

        _context.Events.Remove(@event);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.EventID == id);
    }
}
