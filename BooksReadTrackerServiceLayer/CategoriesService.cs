using BooksReadTrackerDatabaseLayer;
using BooksReadTrackerModels;

namespace BooksReadTrackerServiceLayer;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoriesRepository _categoryRepository;

    public CategoriesService(ICategoriesRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public Task<List<Category>> GetAllAsync()
    {
        return _categoryRepository.GetAllAsync();
    }

    public Task<Category?> GetAsync(int id)
    {
        return _categoryRepository.GetAsync(id);
    }
    public Task<int> AddOrUpdateAsync(Category category)
    {
        return _categoryRepository.AddOrUpdateAsync(category);
    }

    public Task<int> DeleteAsync(Category category)
    {
        return _categoryRepository.DeleteAsync(category);
    }

    public Task<int> DeleteAsync(int id)
    {
        return _categoryRepository.DeleteAsync(id);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _categoryRepository.ExistsAsync(id);
    }


}
