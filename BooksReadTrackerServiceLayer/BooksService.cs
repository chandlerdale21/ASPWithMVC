using BooksReadTrackerDatabaseLayer;
using BooksReadTrackerModels;

namespace BooksReadTrackerServiceLayer;

public class BooksService : IBooksServices
{
    private IBooksRepository _booksRepository;


    public BooksService(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }



    Task<int> IBooksServices.AddOrUpdateAsync(Book book, string userId)
    {
        return _booksRepository.AddOrUpdateAsync(book, userId);
    }

    Task<int> IBooksServices.DeleteAsync(Book book, string userId)
    {
        return _booksRepository.DeleteAsync(book, userId);
    }

    Task<int> IBooksServices.DeleteAsync(int id, string userId)
    {
        return _booksRepository.DeleteAsync(id, userId);
    }

    Task<bool> IBooksServices.ExistsAsync(int id, string userId)
    {
        return _booksRepository.ExistsAsync(id, userId);
    }

    Task<List<Book>> IBooksServices.GetAllAsync(string userId)
    {
        return _booksRepository.GetAllAsync(userId);
    }

    Task<Book?> IBooksServices.GetAsync(int id, string userId)
    {
        return _booksRepository.GetAsync(id, userId);
    }
}
