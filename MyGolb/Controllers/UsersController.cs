using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyGolb.Data;
using MyGolb.Models;

namespace MyGolb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    
    [EnableCors("MyGolbPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly MyGolbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(MyGolbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        
        // GET: api/Users/username
        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.User == null)
          {
              return Problem("Entity set 'MyGolbContext.User'  is null.");
          }
          
          user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
          _context.User.Add(user);
          await _context.SaveChangesAsync();
          var token = GenerateJwtToken(user.Username, user.Id);
          return Ok(new { Token = token });
        }
        
        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            var userDB= _context.User.FirstOrDefault(u => u.Username == user.Username);


            if (_context.User == null)
            {
                return Problem("Entity set 'MyGolbTestContext.User'  is null.");
            }

            if (user == null)
                return BadRequest("Invalid login attempt");

            if (!BCrypt.Net.BCrypt.Verify(user.Password, userDB.Password))
                return Unauthorized();
            var token = GenerateJwtToken(userDB.Username, userDB.Id);
            return Ok(new { Token = token });
        }

        // DELETE: api/Users/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        
        private string GenerateJwtToken(string username, long userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim("id", userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
