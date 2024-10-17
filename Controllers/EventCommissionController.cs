using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using DebraSheru.Data;
using DebraSheru.Models;

[Route("api/[controller]")]
[ApiController]
public class EventCommissionController : ControllerBase
{
    private readonly EventManagementContext _context;
    private readonly IConfiguration _configuration;

    public EventCommissionController(EventManagementContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET: api/EventCommission/GetEventCommissions
    [HttpGet("GetEventCommissions")]
    [Authorize(Roles = "Admin, Partner")]
    public async Task<ActionResult<IEnumerable<object>>> GetEventCommissions()
    {
        var eventCommissions = await _context.EventCommissions
            .Include(ec => ec.Event) 
            .Select(ec => new
            {
                EventID = ec.EventID,
                CommissionRate = ec.CommissionRate,
                EventName = ec.Event.EventName
            })
            .ToListAsync();

        if (eventCommissions == null || !eventCommissions.Any())
        {
            return NotFound();
        }

        return Ok(eventCommissions);
    }

    // GET: api/EventCommission/GetEventCommissionById/{id}
    [HttpGet("GetEventCommissionById/{id}")]
    [Authorize(Roles = "Admin, Partner")]
    public async Task<IActionResult> GetEventCommission(int id)
    {
        var eventCommission = await _context.EventCommissions
            .Include(ec => ec.Event) 
            .Where(ec => ec.EventID == id)
            .Select(ec => new
            {
                EventID = ec.EventID,
                CommissionRate = ec.CommissionRate,
                EventName = ec.Event.EventName
            })
            .FirstOrDefaultAsync();

        if (eventCommission == null)
        {
            return NotFound();
        }

        return Ok(eventCommission);
    }

    // POST: api/EventCommission/CreateEventCommission
    [HttpPost("CreateEventCommission")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventCommission>> CreateEventCommission(EventCommission eventCommission)
    {
        _context.EventCommissions.Add(eventCommission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEventCommission), new { id = eventCommission.EventID }, eventCommission);
    }

    // PUT: api/EventCommission/UpdateEventCommission/{id}
    [HttpPut("UpdateEventCommission/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEventCommission(int id, EventCommission eventCommission)
    {
        if (id != eventCommission.EventID)
        {
            return BadRequest();
        }

        _context.Entry(eventCommission).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventCommissionExists(id))
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

    // DELETE: api/EventCommission/DeleteEventCommission/{id}
    [HttpDelete("DeleteEventCommission/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEventCommission(int id)
    {
        var eventCommission = await _context.EventCommissions.FindAsync(id);
        if (eventCommission == null)
        {
            return NotFound();
        }

        _context.EventCommissions.Remove(eventCommission);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/EventCommission/GetEventCommissionsByEventID/{eventId}
    [HttpGet("GetEventCommissionsByEventID/{eventId}")]
    [Authorize(Roles = "Admin, Partner")]
    public async Task<ActionResult<object>> GetEventCommissionsByEvent(int eventId)
    {
        var eventCommission = await _context.EventCommissions
            .Include(ec => ec.Event) 
            .Where(ec => ec.EventID == eventId)
            .Select(ec => new
            {
                EventID = ec.EventID,
                CommissionRate = ec.CommissionRate,
                EventName = ec.Event.EventName
            })
            .FirstOrDefaultAsync();

        if (eventCommission == null)
        {
            return NotFound();
        }

        return Ok(eventCommission);
    }

    private bool EventCommissionExists(int id)
    {
        return _context.EventCommissions.Any(e => e.EventID == id);
    }
}
