using DebraSheru.Data;
using DebraSheru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly EventManagementContext _context;
    private readonly IConfiguration _configuration;

    public UserController(EventManagementContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // POST: api/User/Register
    [HttpPost("Register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> Register(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest("Invalid user data.");
        }

        user.UserID = 0;

        bool userExists = await _context.Users.AnyAsync(p => p.Email == user.Email);
        if (userExists)
        {
            return BadRequest("This user already exists.");
        }

        // Hash password
        user.Password = HashPassword(user.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    // POST: api/User/Login
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Invalid login data.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.Password == loginRequest.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.Role) 
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    // PUT: api/User/UpdateUser
    [HttpPut("UpdateUser")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> UpdateUser(User updatedUser)
    {
        if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Email) || string.IsNullOrEmpty(updatedUser.Password))
        {
            return BadRequest("Invalid user data.");
        }

        // Check if the user exists
        var user = await _context.Users.FindAsync(updatedUser.UserID);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Check if the email is being changed to an existing email
        bool emailExists = await _context.Users.AnyAsync(u => u.Email == updatedUser.Email && u.UserID != updatedUser.UserID);
        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }

        // Update user properties
        user.Email = updatedUser.Email;
        user.Password = HashPassword(updatedUser.Password); 
        user.Role = updatedUser.Role;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    // GET: api/User/ViewAllUsers
    [HttpGet("ViewAllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<User>>> ViewAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // DELETE: api/User/DeleteUser/{id}
    [HttpDelete("DeleteUser/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        // Check if the user exists
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Remove the user from the database
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }

    // POST: api/User/CreateAdmin
    [HttpPost("CreateAdmin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> CreateAdmin(User admin)
    {
        if (admin == null || string.IsNullOrEmpty(admin.Email) || string.IsNullOrEmpty(admin.Password))
        {
            return BadRequest("Invalid admin data.");
        }

        admin.UserID = 0;

        bool userExists = await _context.Users.AnyAsync(u => u.Email == admin.Email);
        if (userExists)
        {
            return BadRequest("Admin already exists.");
        }

        
        admin.Password = HashPassword(admin.Password);
        admin.Role = "Admin";

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();
        return Ok(admin);
    }

    private string HashPassword(string password)
    {
        return password;
    }
}
