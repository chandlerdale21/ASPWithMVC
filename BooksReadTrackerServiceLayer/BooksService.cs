using BooksReadTrackerModels;

namespace BooksReadTrackerServiceLayer;

public class BooksService : IBooksServices
{
    private readonly IBooksServices _booksRepository;

    Task<int> IBooksServices.AddOrUpdateAsync(Book book)
    {
        return _booksRepository.AddOrUpdateAsync(book);
    }

    Task<int> IBooksServices.DeleteAsync(Book book)
    {
        return _booksRepository.DeleteAsync(book);
    }

    Task<int> IBooksServices.DeleteAsync(int id)
    {
        return _booksRepository.DeleteAsync(id);
    }

    Task<bool> IBooksServices.ExistsAsync(int id)
    {
        return _booksRepository.ExistsAsync(id);
    }

    Task<List<Book>> IBooksServices.GetAllAsync()
    {
        return _booksRepository.GetAllAsync();
    }

    Task<Book?> IBooksServices.GetAsync(int id)
    {
        return _booksRepository.GetAsync(id);
    }
}
