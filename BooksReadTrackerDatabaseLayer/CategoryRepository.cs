using BooksReadTrackerModels;

namespace BooksReadTrackerDatabaseLayer
{
    public class CategoryRepository : ICategoriesRepository
    {
        public Task<List<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Category?> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddOrUpdateAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
