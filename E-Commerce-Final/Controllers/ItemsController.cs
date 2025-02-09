using Microsoft.AspNetCore.Mvc;
using E_Commerce_Final.Data;
using E_Commerce_Final.Models;
using E_Commerce_Final.Services;

namespace E_Commerce_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IItemsService _itemsService;

        public ItemsController(AppDbContext context, IItemsService itemsService)
        {
            _context = context;
            _itemsService = itemsService;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _itemsService.GetItems();
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(Guid id)
        {
            var item = await _itemsService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem([FromForm] Item item)
        {
            var result = await _itemsService.Save(item);
            if (result == "Success")
            {
                return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
            }
            return BadRequest(result);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(Guid id, [FromBody] Item item)
        {
            if (id != item.ItemId)
            {
                return BadRequest();
            }

            var result = await _itemsService.Update(id, item);
            if (result == "Item not found")
            {
                return NotFound();
            }

            return NoContent();
        }
         
        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _itemsService.Delete(id);
            if (result == "Item not found")
            {
                return NotFound();
            }

            return NoContent();
        }

        // SEARCH: api/Items/search?name=example
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Item>>> SearchItems([FromQuery] string name)
        {
            var items = await _itemsService.Search(name);
            return Ok(items);
        }
    }
}
