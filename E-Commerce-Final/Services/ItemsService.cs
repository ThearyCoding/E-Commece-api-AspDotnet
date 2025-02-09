using E_Commerce_Final.Data;
using E_Commerce_Final.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Final.Services
{
    public class ItemsService : IItemsService, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _web;

        public ItemsService(AppDbContext context, IWebHostEnvironment web)
        {
            _context = context;
            _web = web;
        }

        public void Dispose() => _context.Dispose();

        // DELETE: Remove an item by ID
        public async Task<string> Delete(Guid Id)
        {
            var item = await _context.Items.Include(i => i.Images).FirstOrDefaultAsync(i => i.ItemId == Id);
            if (item == null)
            {
                return "Item not found";
            }

            // Delete associated images
            foreach (var image in item.Images)
            {
                var imagePath = Path.Combine(_web.WebRootPath, "Uploads", image.Image);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            _context.ItemImages.RemoveRange(item.Images); // Remove images from DB
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return "Deleted successfully";
        }

        public async Task<Item> GetItem(Guid Id)
        {
            return await _context.Items.Include(x => x.Images).FirstOrDefaultAsync(x => x.ItemId == Id);
        }

        // GET: Get all items
        public async Task<List<Item>> GetItems()
            => await _context.Items.Include(x => x.Images).ToListAsync();

        // POST: Save a new item
        public async Task<string> Save(Item item)
        {
            try
            {
                item.ItemId = Guid.NewGuid();
                _context.Items.Add(item);
                await _context.SaveChangesAsync(); // Ensure the item exists in DB before adding images

                if (item.Files?.Count > 0)
                {
                    await UploadFile(item.Files, item.ItemId);
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // GET: Search for items by name
        public async Task<List<Item>> Search(string name)
        {
            return await _context.Items
                .Where(x => x.ItemName.Contains(name))
                .Include(x => x.Images)
                .ToListAsync();
        }

        // PUT: Update an existing item
        public async Task<string> Update(Guid Id, Item item)
        {
            var existingItem = await _context.Items.Include(i => i.Images).FirstOrDefaultAsync(i => i.ItemId == Id);
            if (existingItem == null)
            {
                return "Item not found";
            }

            existingItem.ItemName = item.ItemName;
            existingItem.Description = item.Description;
            existingItem.Price = item.Price;
            existingItem.QtyOnHand = item.QtyOnHand;
            existingItem.IsActive = item.IsActive;

            await _context.SaveChangesAsync();

            // Handle file uploads
            if (item.Files?.Count > 0)
            {
                await UploadFile(item.Files, existingItem.ItemId);
            }

            return "Updated successfully";
        }

        // CHECK: If item name exists
        public async Task<bool> IsNameExist(string name)
            => await _context.Items.AnyAsync(x => x.ItemName.Equals(name));

        public async Task<string> UploadFile(List<IFormFile>? files, Guid itemId)
        {
            if (files == null || files.Count == 0)
            {
                return "No files uploaded";
            }

            try
            {
                var dir = Path.Combine(_web.WebRootPath, "Uploads");

                // Ensure directory exists
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                foreach (var file in files)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var fullPath = Path.Combine(dir, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var image = new ItemImage
                    {
                        ItemImageId = Guid.NewGuid(),
                        Image = fileName,
                        ItemId = itemId
                    };

                    _context.ItemImages.Add(image);
                }

                await _context.SaveChangesAsync();
                return "Upload successful";
            }
            catch (Exception ex)
            {
                return $"Error uploading file: {ex.Message}";
            }
        }
    }
}
