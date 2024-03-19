using BooksReadTrackerModels;

namespace BooksReadTrackerDatabaseLayer
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetAsync(int id);

        Task<int> AddOrUpdateAsync(Category category);

        Task<int> DeleteAsync(Category category);

        Task<int> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
