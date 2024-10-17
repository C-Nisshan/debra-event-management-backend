using DebraSheru.Data;
using DebraSheru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly EventManagementContext _context;

    public TicketController(EventManagementContext context)
    {
        _context = context;
    }

    // GET: api/Ticket
    [HttpGet("GetAllTickets")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        return await _context.Tickets.ToListAsync();
    }

    // GET: api/Ticket/{id}
    [HttpGet("GetTicketById/{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    // POST: api/Ticket/CreateBatch
    [HttpPost("CreateTicketBatch")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketBatch>> CreateTicketBatch(TicketBatch ticketBatch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.TicketBatches.Add(ticketBatch);
                await _context.SaveChangesAsync();

                // Retrieve the TicketBatchID after saving
                var ticketBatchId = ticketBatch.TicketBatchID;

                var tickets = Enumerable.Range(1, ticketBatch.TotalTickets).Select(_ => new Ticket
                {
                    EventID = ticketBatch.EventID,
                    TicketTypeID = ticketBatch.TicketTypeID,
                    TicketBatchID = ticketBatchId, // Set the TicketBatchID
                    Price = ticketBatch.Price,
                    Status = "Available"
                }).ToList();

                _context.Tickets.AddRange(tickets);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetTicketBatch), new { id = ticketBatchId }, ticketBatch);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception details
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"An error occurred while creating the ticket batch: {innerExceptionMessage}");
            }
        }
    }

    // GET: api/Ticket/GetAllTicketBatches
    [HttpGet("GetAllTicketBatches")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<TicketBatchResponseDto>>> GetAllTicketBatches()
    {
        var ticketBatches = await _context.TicketBatches
            .Include(tb => tb.Event)
            .Include(tb => tb.TicketType)
            .ToListAsync();

        if (ticketBatches == null || !ticketBatches.Any())
        {
            return NotFound();
        }

        var response = ticketBatches.Select(tb => new TicketBatchResponseDto
        {
            TicketBatchID = tb.TicketBatchID,
            EventName = tb.Event.EventName,
            TicketTypeName = tb.TicketType.TicketTypeName,
            TotalTickets = tb.TotalTickets,
            Price = tb.Price
        }).ToList();

        return Ok(response);
    }




    // GET: api/Ticket/{id}
    [HttpGet("GetTicketBatchById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketBatch>> GetTicketBatch(int id)
    {
        //var ticketBatch = await _context.TicketBatches.FindAsync(id);

        var ticketBatch = await _context.TicketBatches
        .Include(tb => tb.Event)
        .Include(tb => tb.TicketType)
        .FirstOrDefaultAsync(tb => tb.TicketBatchID == id);


        if (ticketBatch == null)
        {
            return NotFound();
        }

        var response = new TicketBatchResponseDto
        {
            TicketBatchID = ticketBatch.TicketBatchID,
            EventName = ticketBatch.Event.EventName,
            TicketTypeName = ticketBatch.TicketType.TicketTypeName,
            TotalTickets = ticketBatch.TotalTickets,
            Price = ticketBatch.Price
        };

        return Ok(response);
    }

    // PUT: api/Ticket/UpdateBatch/{id}
    [HttpPut("UpdateBatch/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTicketBatch(int id, TicketBatchUpdateDto updateDto)
    {
        if (id != updateDto.TicketBatchID)
        {
            return BadRequest("ID mismatch");
        }

        var ticketBatch = await _context.TicketBatches.FindAsync(id);

        if (ticketBatch == null)
        {
            return NotFound();
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                ticketBatch.EventID = updateDto.EventID;
                ticketBatch.TicketTypeID = updateDto.TicketTypeID;
                ticketBatch.TotalTickets = updateDto.TotalTickets;
                ticketBatch.Price = updateDto.Price;

                _context.TicketBatches.Update(ticketBatch);
                await _context.SaveChangesAsync();

                var existingTickets = await _context.Tickets
                    .Where(t => t.TicketBatchID == id)
                    .ToListAsync();

                if (updateDto.TotalTickets > existingTickets.Count)
                {
                    var additionalTickets = Enumerable.Range(1, updateDto.TotalTickets - existingTickets.Count)
                        .Select(_ => new Ticket
                        {
                            EventID = updateDto.EventID,
                            TicketTypeID = updateDto.TicketTypeID,
                            TicketBatchID = id,
                            Price = updateDto.Price,
                            Status = "Available"
                        }).ToList();

                    _context.Tickets.AddRange(additionalTickets);
                }
                else if (updateDto.TotalTickets < existingTickets.Count)
                {
                    var ticketsToRemove = existingTickets
                        .OrderBy(t => t.TicketID)
                        .Take(existingTickets.Count - updateDto.TotalTickets)
                        .ToList();

                    _context.Tickets.RemoveRange(ticketsToRemove);
                }

                foreach (var ticket in existingTickets)
                {
                    ticket.EventID = updateDto.EventID;
                    ticket.TicketTypeID = updateDto.TicketTypeID;
                    ticket.Price = updateDto.Price;
                }

                _context.Tickets.UpdateRange(existingTickets);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception details
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"An error occurred while updating the ticket batch: {innerExceptionMessage}");
            }
        }
    }


    // DELETE: api/Ticket/DeleteBatch/{id}
    [HttpDelete("DeleteBatch/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTicketBatch(int id)
    {
        var ticketBatch = await _context.TicketBatches.FindAsync(id);

        if (ticketBatch == null)
        {
            return NotFound();
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var tickets = await _context.Tickets
                    .Where(t => t.TicketBatchID == id)
                    .ToListAsync();

                _context.Tickets.RemoveRange(tickets);
                _context.TicketBatches.Remove(ticketBatch);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception details
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"An error occurred while deleting the ticket batch: {innerExceptionMessage}");
            }
        }
    }


    // DELETE: api/Ticket/{id}
    [HttpDelete("DeleteTicketById/{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TicketExists(int id)
    {
        return _context.Tickets.Any(t => t.TicketID == id);
    }
}
