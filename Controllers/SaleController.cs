using DebraSheru.Data;
using DebraSheru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly EventManagementContext _context;
    private readonly ILogger<SaleController> _logger;

    public SaleController(ILogger<SaleController> logger, EventManagementContext context)
    {
        _context = context;
        _logger = logger;
    }


    // POST: api/Sale/CreateSale
    [HttpPost("CreateSale")]
    //[Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<Sale>> CreateSale(Sale sale)
    {
        
        // Retrieve and validate the user's role and ID from the claims
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Log the retrieved values for debugging
        _logger.LogInformation("User Role: {UserRole}", userRole);
        _logger.LogInformation("User ID (string): {UserIdString}", userIdString);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user identifier.");
        }

        if (userRole == "Admin" || userRole == "Partner")
        {
            
        }

        return Unauthorized();
    }
    /*
        // GET: api/Sale
        [HttpGet("GetSales")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin")
            {
                return await _context.Sales.Include(s => s.User).Include(s => s.Ticket).ThenInclude(t => t.Event).ToListAsync();
            }
            else if (userRole == "Partner")
            {
                return await _context.Sales
                    .Where(s => s.UserID == userId)
                    .Include(s => s.User)
                    .Include(s => s.Ticket)
                    .ThenInclude(t => t.Event)
                    .ToListAsync();
            }

            return Unauthorized();
        }
    */

    // GET: api/Sale
    [HttpGet("GetSales")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        try
        {
            // No need to check user role or user ID since both Admin and Partner can view all sales
            return await _context.Sales.Include(s => s.User).Include(s => s.Ticket).ThenInclude(t => t.Event).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting sales.");
            return StatusCode(500, "Internal server error");
        }
    }
    /*
        // GET: api/Sale/{id}
        [HttpGet("GetSalesById{id}")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _context.Sales.Include(s => s.User).Include(s => s.Ticket).ThenInclude(t => t.Event).FirstOrDefaultAsync(s => s.SaleID == id);

            if (sale == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin" || (userRole == "Partner" && sale.UserID == userId))
            {
                return sale;
            }

            return Unauthorized();
        }
    */

    // GET: api/Sale/{id}
    [HttpGet("GetSalesById{id}")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        try
        {
            var sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.Ticket)
                .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(s => s.SaleID == id);

            if (sale == null)
            {
                return NotFound();
            }
            else
            {
                return sale;
            }

            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the sale.");
            return StatusCode(500, "Internal server error");
        }
    }
    /*
        // Filter and categorize sales
        [HttpGet("FilterByEvent/{eventId}")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<ActionResult<IEnumerable<Sale>>> FilterSalesByEvent(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin")
            {
                return await _context.Sales
                    .Where(s => s.Ticket.EventID == eventId)
                    .Include(s => s.User)
                    .Include(s => s.Ticket)
                    .ThenInclude(t => t.Event)
                    .ToListAsync();
            }
            else if (userRole == "Partner")
            {
                return await _context.Sales
                    .Where(s => s.Ticket.EventID == eventId && s.UserID == userId)
                    .Include(s => s.User)
                    .Include(s => s.Ticket)
                    .ThenInclude(t => t.Event)
                    .ToListAsync();
            }

            return Unauthorized();
        }
    */

    // GET: api/Sale/FilterByEvent/{eventId}
    [HttpGet("FilterByEvent/{eventId}")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<IEnumerable<Sale>>> FilterSalesByEvent(int eventId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Sale> query = _context.Sales
                .Where(s => s.Ticket.EventID == eventId)
                .Include(s => s.User)
                .Include(s => s.Ticket)
                .ThenInclude(t => t.Event);

            
            return await query.ToListAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering sales by event.");
            return StatusCode(500, "Internal server error");
        }
    }

    /*
        [HttpGet("FilterByTicketType/{eventId}/{ticketTypeId}")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<ActionResult<IEnumerable<Sale>>> FilterSalesByTicketType(int eventId, int ticketTypeId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin")
            {
                return await _context.Sales
                    .Where(s => s.Ticket.EventID == eventId && s.Ticket.TicketTypeID == ticketTypeId)
                    .Include(s => s.User)
                    .Include(s => s.Ticket)
                    .ThenInclude(t => t.Event)
                    .ToListAsync();
            }
            else if (userRole == "Partner")
            {
                return await _context.Sales
                    .Where(s => s.Ticket.EventID == eventId && s.Ticket.TicketTypeID == ticketTypeId && s.UserID == userId)
                    .Include(s => s.User)
                    .Include(s => s.Ticket)
                    .ThenInclude(t => t.Event)
                    .ToListAsync();
            }

            return Unauthorized();
        }
    */

    // GET: api/Sale/FilterByTicketType/{eventId}/{ticketTypeId}
    [HttpGet("FilterByTicketType/{eventId}/{ticketTypeId}")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<IEnumerable<Sale>>> FilterSalesByTicketType(int eventId, int ticketTypeId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Sale> query = _context.Sales
                .Where(s => s.Ticket.EventID == eventId && s.Ticket.TicketTypeID == ticketTypeId)
                .Include(s => s.User)
                .Include(s => s.Ticket)
                .ThenInclude(t => t.Event);

            return await query.ToListAsync();
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering sales by ticket type.");
            return StatusCode(500, "Internal server error");
        }
    }

/*
    [HttpGet("SalesByNIC/{nicNumber}")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSalesByNIC(string nicNumber)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole == "Admin")
        {
            return await _context.Sales
                .Where(s => s.NICNumber == nicNumber)
                .Include(s => s.User)
                .Include(s => s.Ticket)
                .ThenInclude(t => t.Event)
                .ToListAsync();
        }
        else if (userRole == "Partner")
        {
            return await _context.Sales
                .Where(s => s.NICNumber == nicNumber && s.UserID == userId)
                .Include(s => s.User)
                .Include(s => s.Ticket)
                .ThenInclude(t => t.Event)
                .ToListAsync();
        }

        return Unauthorized();
    }
*/

    private bool SaleExists(int id)
    {
        return _context.Sales.Any(e => e.SaleID == id);
    }
}
