using BooksReadTrackerModels;

namespace BooksReadTrackerServiceLayer
{
    public interface ICategoriesService
    {

        Task<List<Category>> GetAllAsync();
        Task<Category?> GetAsync(int id);

        Task<int> AddOrUpdateAsync(Category category);

        Task<int> DeleteAsync(Category category);

        Task<int> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
