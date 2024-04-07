using BooksReadTrackerModels;
namespace BooksReadTrackerDatabaseLayer;

public interface IBooksReadOnlyRepository
{
    Task<List<Book>> GetAllAsync(string userId);
    Task<Book?> GetAsync(int id, string userId);

    Task<bool> ExistsAsync(int id, string userId);
}

public interface IBooksWriteOnlyRepository
{
    Task<int> AddOrUpdateAsync(Book book, string userId);
    Task<int> DeleteAsync(Book book, string userId);
    Task<int> DeleteAsync(int id, string userId);
}

public interface IBooksRepository : IBooksReadOnlyRepository, IBooksWriteOnlyRepository
{

}