using BooksReadTrackerModels;
namespace BooksReadTrackerDatabaseLayer;

public interface IBooksRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetAsync(int id);
    Task<int> AddOrUpdateAsync(Book book);
    Task<int> DeleteAsync(Book book);
    Task<int> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
