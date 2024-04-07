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

        public async Task<List<Book>> GetAllAsync(string userId)
        {
            return await _context.Books
                .Include(x => x.Category)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
        public async Task<Book?> GetAsync(int id, string userId)
        {
            return await _context.Books.Include(x => x.Category).Where(x => x.UserId == userId).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> AddOrUpdateAsync(Book book, string userId)
        {
            if (book == null)
            {
                throw new ArgumentException("Book cannot be null.", nameof(book));
            }
            if (book.Id < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid Id");
            }

            if (string.IsNullOrWhiteSpace(userId)
           || string.IsNullOrWhiteSpace(book.UserId)
           || !book.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentNullException("UserId mismatch or unset on add/edit");
            }

            //make sure category is valid
            var goodCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Id == book.CategoryId);
            if (goodCategory is null)
            {
                throw new ArgumentOutOfRangeException("Category not found");
            }

            //add or update
            if (book.Id > 0)
            {
                return await Update(book);
            }
            else
            {
                return await Add(book);
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
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task<int> DeleteAsync(Book book, string userId)
        {
            ArgumentNullException.ThrowIfNull(book, "Item cannot be null");
            return await DeleteAsync(book.Id, userId);

        }

        public async Task<int> DeleteAsync(int id, string userId)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(id, "Invalid Id");
            ArgumentOutOfRangeException.ThrowIfZero(id, "Invalid Id");
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, "UserId cannot be null or empty");

            var existing = await _context.Books.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            ArgumentNullException.ThrowIfNull(existing, "Item not found");

            _context.Books.Remove(existing);
            await _context.SaveChangesAsync();
            return existing.Id;
        }

        public async Task<bool> ExistsAsync(int id, string userId)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(id, "Invalid Id");
            ArgumentOutOfRangeException.ThrowIfZero(id, "Invalid Id");
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, "UserId cannot be null or empty");

            return await _context.Books.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId) != null;
        }
    }
}
