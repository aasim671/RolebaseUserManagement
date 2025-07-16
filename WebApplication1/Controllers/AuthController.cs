using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // <--- THIS LINE IS CRUCIAL AND WAS MISSING!
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entity; // This brings WebApplication1.Entity.User into scope
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        // Explicitly specify the User type for IPasswordHasher
        private readonly IPasswordHasher<WebApplication1.Entity.User> _passwordHasher;

        public AuthController(AppDbContext context, IPasswordHasher<WebApplication1.Entity.User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register dto)
        {
            // Check if username already exists  
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            // Create new user  
            var user = new WebApplication1.Entity.User // Explicitly use the full namespace for clarity
            {
                Username = dto.Username,
                Email = dto.Email
            };

            // Hash password  
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            // Save to DB  
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            // Find user  
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            // Verify password  
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) // PasswordVerificationResult also needs this using statement
                return Unauthorized("Invalid username or password.");
            return Ok(new { message = "Login successful." });
        }
    }
}