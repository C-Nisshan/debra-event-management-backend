using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DebraSheru.Data;
using DebraSheru.Models;

namespace Debra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LocationController : ControllerBase
    {
        private readonly EventManagementContext _context;

        public LocationController(EventManagementContext context)
        {
            _context = context;
        }

        // GET: api/Location/GeAllLocations
        [HttpGet("GeAllLocations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetAllLocations()
        {
            return await _context.Locations.ToListAsync();
        }

        // GET: api/Location/GetLocationByID/5
        [HttpGet("GetLocationByID/{id}")]
        public async Task<ActionResult<Location>> GetLocationByID(int id)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        // PUT: api/Location/UpdateLocationByID/5
        [HttpPut("UpdateLocationByID/{id}")]
        public async Task<IActionResult> UpdateLocationByID(int id, Location location)
        {
            if (id != location.LocationID)
            {
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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

        // POST: api/Location/CreateLocation
        [HttpPost("CreateLocation")]
        public async Task<ActionResult<Location>> CreateLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocationByID), new { id = location.LocationID }, location);
        }

        // DELETE: api/Location/DeleteLocation/5
        [HttpDelete("DeleteLocationById/{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}
