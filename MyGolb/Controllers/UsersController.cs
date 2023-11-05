using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UsersController(MyGolbContext context)
        {
            _context = context;
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

          return CreatedAtAction("GetUser", new { id = user.Id }, user);
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

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
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
    }
}
