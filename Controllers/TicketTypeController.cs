using DebraSheru.Data;
using DebraSheru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TicketTypeController : ControllerBase
{
    private readonly EventManagementContext _context;

    public TicketTypeController(EventManagementContext context)
    {
        _context = context;
    }

    // GET: api/TicketType
    [HttpGet("GetTicketType")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<TicketType>>> GetTicketTypes()
    {
        return await _context.TicketTypes.ToListAsync();
    }

    // GET: api/TicketType/{id}
    [HttpGet("GetTicketTypeBiId/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketType>> GetTicketType(int id)
    {
        var ticketType = await _context.TicketTypes.FindAsync(id);

        if (ticketType == null)
        {
            return NotFound();
        }

        return Ok(ticketType);
    }

    // POST: api/TicketType
    [HttpPost("CreateTicketType")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketType>> CreateTicketType(TicketType ticketType)
    {
        _context.TicketTypes.Add(ticketType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicketType), new { id = ticketType.TicketTypeID }, ticketType);
    }

    // PUT: api/TicketType/{id}
    [HttpPut("UpdateTicketTypeById{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTicketType(int id, TicketType ticketType)
    {
        if (id != ticketType.TicketTypeID)
        {
            return BadRequest();
        }

        _context.Entry(ticketType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TicketTypeExists(id))
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

    // DELETE: api/TicketType/{id}
    [HttpDelete("DeleteTicketTypeById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTicketType(int id)
    {
        var ticketType = await _context.TicketTypes.FindAsync(id);
        if (ticketType == null)
        {
            return NotFound();
        }

        _context.TicketTypes.Remove(ticketType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TicketTypeExists(int id)
    {
        return _context.TicketTypes.Any(tt => tt.TicketTypeID == id);
    }
}
