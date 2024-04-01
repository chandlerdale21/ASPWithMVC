using BooksReadTrackerDBLibrary;
using BooksReadTrackerModels;
using Microsoft.EntityFrameworkCore;

namespace BooksReadTrackerDatabaseLayer
{
    public class CategoryRepository : ICategoriesRepository
    {
        private readonly BooksReadTrackerDbContext _context;

        public CategoryRepository(BooksReadTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {

            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<int> AddOrUpdateAsync(Category category)
        {
            if (category.Id == 0)
            {
                _context.Categories.Add(category);
            }
            else
            {
                var dbCategory = await _context.Categories.FindAsync(category.Id);
                if (dbCategory != null)
                {
                    dbCategory.Name = category.Name;
                    _context.Categories.Update(dbCategory);
                }
                else
                {
                    // Handle the case where the category doesn't exist.
                    return 0; // Or throw an exception.
                }
            }
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> DeleteAsync(Category category)
        {
            return await DeleteAsync(category.Id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return id;
            }
            return 0; // Or handle the case where the category doesn't exist.
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(m => m.Id == id) != null;
        }
    }
}
