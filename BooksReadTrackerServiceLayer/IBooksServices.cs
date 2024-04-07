using BooksReadTrackerModels;

namespace BooksReadTrackerServiceLayer
{
    public interface IBooksServices
    {
        Task<List<Book>> GetAllAsync(string userId);
        Task<Book?> GetAsync(int id, string userId);
        Task<int> AddOrUpdateAsync(Book book, string userId);
        Task<int> DeleteAsync(Book book, string userId);
        Task<int> DeleteAsync(int id, string userId);
        Task<bool> ExistsAsync(int id, string userId);
    }
}
