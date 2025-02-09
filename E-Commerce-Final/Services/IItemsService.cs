using E_Commerce_Final.Models;

namespace E_Commerce_Final.Services
{
    public interface IItemsService
    {
        Task<List<Item>> GetItems();
        Task<Item> GetItem(Guid Id);
        Task<string> Save(Item item);
        Task<string> Update(Guid Id, Item item);
        Task<string> Delete(Guid Id);
        Task<List<Item>> Search(string name);
        Task<bool> IsNameExist(string name);
        Task<string> UploadFile(List<IFormFile>? files,Guid itemId);
    }
}
