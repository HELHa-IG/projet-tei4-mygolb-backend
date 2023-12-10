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
using MyGolb.Enums;
using MyGolb.Models;

namespace MyGolb.Controllers
{
    [EnableCors("MyGolbPolicy")]
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly MyGolbContext _context;
        private readonly string _uploadPath;

        public PostsController(MyGolbContext context, IConfiguration configuration)
        {
            _context = context;
            _uploadPath = configuration["UploadSettings:PostPath"];

        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
          if (_context.Post == null)
          {
              return NotFound();
          }
            return await _context.Post.Include(pst => pst.User).ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(long id)
        {
          if (_context.Post == null)
          {
              return NotFound();
          }
            var post = await _context.Post.Include(pst => pst.User).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(long id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(IFormFile file)
        {
          if (_context.Post == null || file.Length == 0)
          {
              return Problem("Entity set 'MyGolbContext.Post'  is null.");
          }
          var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
          var fullPath = Path.Combine(_uploadPath, filename);
          
          if (_context.User == null)
          {
              return NotFound("There is no context");
          }
          
          var userIdClaim = User.Claims.FirstOrDefault(claim => claim.Type == "id");

          if (userIdClaim == null)
          {
              return BadRequest("User ID claim not found.");
          }

          long userId;
          if (!long.TryParse(userIdClaim.Value, out userId))
          {
              return BadRequest("User ID claim is not a valid long.");
          }
          
          var user = await _context.User.FindAsync(userId);

          if (user == null)
          {
              return NotFound("There is no user with this ID");
          }
            
          var ext = Path.GetExtension(filename).ToLower();

          var type =  ext switch
          {
              ".jpg" or ".jpeg" or ".png" or ".gif" => PostType.Image,
              ".mp3" or ".wav" => PostType.Audio,
              ".mp4" or ".avi" or ".mkv" => PostType.Video,
              _ => throw new Exception("Unsupported media type")
          };
            
          var post = new Post
          {
              PostPath = fullPath,
              Type = type,
              Date = DateTime.UtcNow,
              User = user
          };
          
          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
              file.CopyTo(stream);
          }
          
          _context.Post.Add(post);
          await _context.SaveChangesAsync();

          return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            if (_context.Post == null)
            {
                return NotFound();
            }
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(long id)
        {
            return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
