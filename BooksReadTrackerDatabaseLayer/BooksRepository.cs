using BooksReadTrackerDBLibrary;
using BooksReadTrackerModels;

using Microsoft.EntityFrameworkCore;


namespace BooksReadTrackerDatabaseLayer
{
    public class BooksRepository : IBooksRepository
    {

        private readonly BooksReadTrackerDbContext _context;


        public BooksRepository(BooksReadTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(x => x.Category)
                .ToListAsync();
        }
        public async Task<Book?> GetAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            return await _context.Books.Include(x => x.Category).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> AddOrUpdateAsync(Book book)
        {
            if (book == null)
            {
                throw new ArgumentException("Book cannot be null.", nameof(book));
            }

            //var catIdeez = await _context.Categories.Select(x => new {x.Id}).ToListAsync();
            //if(!catIdeez.Contains(book.CategoryId))

            bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == book.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException($"Category with Id {book.CategoryId} does not exist.", nameof(book.CategoryId));
            }

            if (book.Id == 0)
            {
                return await Add(book);
            }
            else
            {
                return await Update(book);
            }
        }
        private async Task<int> Update(Book book)
        {
            var existing = await _context.Books.FirstOrDefaultAsync(x => x.Id == book.Id);
            if (existing == null)
            {
                throw new ArgumentException($"Poster ID {book.Id} Not found");
            }
            existing.Name = book.Name;
            existing.FilePath = book.FilePath;
            existing.CategoryId = book.CategoryId;
            existing.Image = book.Image;

            await _context.SaveChangesAsync();

            return existing.Id;
        }

        private async Task<int> Add(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task<int> DeleteAsync(Book book)
        {
            return await DeleteAsync(book.Id);

        }

        public async Task<int> DeleteAsync(int id)
        {
            var existing = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            _context.Books.Remove(existing);
            await _context.SaveChangesAsync();
            return existing.Id;
        }

        public async Task<bool> ExistsAsync(int id)
        {

            return await _context.Books.FirstOrDefaultAsync(m => m.Id == id) != null;
        }
    }
}
