using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGolb.Data;
using MyGolb.Models;


namespace MyGolb.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly MyGolbContext _context;

        public InteractionsController(MyGolbContext context)
        {
            _context = context;
        }

        // GET: api/Interactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteraction()
        {
          if (_context.Interaction == null)
          {
              return NotFound();
          }
            return await _context.Interaction.ToListAsync();
        }

        // GET: api/Interactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Interaction>> GetInteraction(long id)
        {
          if (_context.Interaction == null)
          {
              return NotFound();
          }
            var interaction = await _context.Interaction.FindAsync(id);

            if (interaction == null)
            {
                return NotFound();
            }

            return interaction;
        }
        
        // GET: api/Interactions/post/5
        [HttpGet("post/{id}")]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractionsByPostId(long id)
        {
            if (_context.Interaction == null && _context.Post == null)
            {
                return NotFound();
            }
            
            return await _context.Interaction.Where(i => i.Post.Id == id).ToListAsync();
        }
        
        // GET: api/Interactions/comment/5
        [HttpGet("comment/{id}")]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractionsByCommentId(long id)
        {
            if (_context.Interaction == null && _context.Comment == null)
            {
                return NotFound();
            }
            
            return await _context.Interaction.Where(i => i.Comment.Id == id).ToListAsync();
        }

        // PUT: api/Interactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInteraction(long id, Interaction interaction)
        {
            if (id != interaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(interaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractionExists(id))
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

        // POST: api/Interactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Interaction>> PostInteraction(Interaction interaction)
        {
          if (_context.Interaction == null)
          {
              return Problem("Entity set 'MyGolbContext.Interaction'  is null.");
          }
            _context.Interaction.Add(interaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInteraction", new { id = interaction.Id }, interaction);
        }

        // DELETE: api/Interactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInteraction(long id)
        {
            if (_context.Interaction == null)
            {
                return NotFound();
            }
            var interaction = await _context.Interaction.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
            }

            _context.Interaction.Remove(interaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InteractionExists(long id)
        {
            return (_context.Interaction?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
