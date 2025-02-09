using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce_Final.Data;
using E_Commerce_Final.Models;

namespace E_Commerce_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemImagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemImagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemImage>>> GetItemImages()
        {
            return await _context.ItemImages.ToListAsync();
        }

        // GET: api/ItemImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemImage>> GetItemImage(Guid id)
        {
            var itemImage = await _context.ItemImages.FindAsync(id);
            if (itemImage == null)
            {
                return NotFound();
            }
            return itemImage;
        }

        // POST: api/ItemImages
        [HttpPost]
        public async Task<ActionResult<ItemImage>> PostItemImage(ItemImage itemImage)
        {
            itemImage.ItemImageId = Guid.NewGuid();
            _context.ItemImages.Add(itemImage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemImage), new { id = itemImage.ItemImageId }, itemImage);
        }

        // PUT: api/ItemImages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemImage(Guid id, ItemImage itemImage)
        {
            if (id != itemImage.ItemImageId)
            {
                return BadRequest();
            }

            _context.Entry(itemImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemImageExists(id))
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

        // DELETE: api/ItemImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemImage(Guid id)
        {
            var itemImage = await _context.ItemImages.FindAsync(id);
            if (itemImage == null)
            {
                return NotFound();
            }

            _context.ItemImages.Remove(itemImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemImageExists(Guid id)
        {
            return _context.ItemImages.Any(e => e.ItemImageId == id);
        }
    }
}
