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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly MyGolbContext _context;
        private readonly string _uploadPath;

        public CommentController(MyGolbContext context, IConfiguration configuration)
        {
            _context = context;
            _uploadPath = configuration["UploadSettings:CommentPath"];
        }

        // GET: api/Comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
          if (_context.Comment == null)
          {
              return NotFound();
          }
            return await _context.Comment.ToListAsync();
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
          if (_context.Comment == null)
          {
              return NotFound();
          }
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }
        
        // GET: api/Comment/Post/5
        [HttpGet("/post/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPostId(long id)
        {
            if (_context.Comment == null && _context.Post == null)
            {
                return NotFound();
            }
            
            return await _context.Comment.Where(c => c.Post.Id == id).ToListAsync();
        }

        // PUT: api/Comment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(long id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(long postId, IFormFile file)
        {
          if (_context.Comment == null || file.Length == 0)
          {
              return Problem("Entity set 'MyGolbContext.Comment'  is null.");
          }
          
            var filename =  $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(_uploadPath, filename);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            
            
            var userIdClaim = User.Claims.FirstOrDefault(claim => claim.Type == "id");
            
            if (_context.User == null)
            {
                return NotFound("There is no context");
            }

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
            var post = await _context.Post.FindAsync(postId);

            if (user == null || post == null)
            {
                return NotFound("There is no user with this ID");
            }
            
            var ext = Path.GetExtension(filename).ToLower();

            var type =  ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" => CommentType.Image,
                ".mp3" or ".wav" => CommentType.Audio,
                ".mp4" or ".avi" or ".mkv" => CommentType.Video,
                _ => throw new Exception("Unsupported media type")
            };

            var comment = new Comment
            {
                CommentPath = fullPath,
                Type = type,
                Date = DateTime.UtcNow,
                User = user,
                Post = post,
            };
            
            
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }
            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(long id)
        {
            return (_context.Comment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
