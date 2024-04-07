using BooksReadTrackerDBLibrary;
using BooksReadTrackerModels;
using BooksReadTrackerServiceLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BooksReadTracker.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly BooksReadTrackerDbContext _context;
        private readonly IBooksServices _booksServices;
        private readonly SelectList _categories;


        public BooksController(BooksReadTrackerDbContext context, IBooksServices booksServices)
        {
            _context = context;
            _categories = new SelectList(_context.Categories, "Id", "Name");
            _booksServices = booksServices;
        }
        //Get: Top10 Books

        public async Task<IActionResult> Top10Index()
        {
            return View(await _context.Books
                .Include(x => x.Category)
                .OrderBy(x => x.Name)
                .Take(10)
                .ToListAsync());
        }

        //GET: Search Result
        public async Task<IActionResult> ShowSearchResult(String SearchPhrase)
        {
            return View("Index", await _context.Books
                .Where(x => x.Name.Contains(SearchPhrase))
                .Include(x => x.Category)
                .ToListAsync());
        }
        //GET: Search
        public async Task<IActionResult> Search()
        {
            return View();
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentUserId();
            var books = await _booksServices.GetAllAsync(userId);
            return View(books ?? new List<Book>());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(x => x.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = _categories;
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FilePath,CategoryId")] Book book)
        {
            if (book.CategoryId is null)
            {
                ModelState.AddModelError("CategoryId", "Category is required");
                ViewData["Categories"] = _categories;
                return View(book);
            }
            var userId = await GetCurrentUserId();
            book.UserId = userId;

            ModelState.Clear();
            TryValidateModel(book);

            if (ModelState.IsValid)
            {
                await _booksServices.AddOrUpdateAsync(book, userId);
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = _categories;
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = _categories;
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FilePath,CategoryId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Books.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = _categories;
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(x => x.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        protected async Task<string> GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

    }
}
