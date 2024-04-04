using BooksReadTracker.Models;
using BooksReadTrackerModels;
using BooksReadTrackerServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BooksReadTracker.Controllers
{
    public class CategoriesController : Controller
    {
        //private readonly BooksReadTrackerDbContext _context;

        private readonly ICategoriesService _categoriesService;
        private IMemoryCache _memoryCache;

        public CategoriesController(ICategoriesService categoriesService, IMemoryCache memoryCache)
        {
            _categoriesService = categoriesService;
            _memoryCache = memoryCache;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await GetCategoriesUsingCache();
            //is the data in the cache? if so, get it from the cache

            //return the data
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var category = await _context.Categories
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var category = await GetSingleCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(category);
                //await _context.SaveChangesAsync();
                await _categoriesService.AddOrUpdateAsync(category);
                InvalidateCache();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        private void InvalidateCache()
        {
            _memoryCache.Remove(CacheConstants.CATEGORIES_KEY);
        }
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var category = await _context.Categories.FindAsync(id);
            var category = await GetSingleCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(category);
                    //await _context.SaveChangesAsync();
                    await _categoriesService.AddOrUpdateAsync(category);
                    InvalidateCache();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var category = await _context.Categories
            //    .FirstOrDefaultAsync(m => m.Id == id);
            Category? category = await GetSingleCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var category = await _context.Categories.FindAsync(id);
            var category = await GetSingleCategoryById(id);
            if (category != null)
            {
                //_context.Categories.Remove(category);
                await _categoriesService.DeleteAsync(category);
            }

            /*await _context.SaveChangesAsync()*/;
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            //return _context.Categories.Any(e => e.Id == id);
            var category = GetSingleCategoryById(id).Result;
            return category != null;
        }
        private async Task<List<Category>?> GetCategoriesUsingCache()
        {
            var categories = new List<Category>();

            //is the data in the cache? if so, get it from the cache
            if (!_memoryCache.TryGetValue(CacheConstants.CATEGORIES_KEY, out categories))
            {
                //if not, get it from the database
                //and set the local list
                categories = await _categoriesService.GetAllAsync();

                //store it in the cache
                _memoryCache.Set(CacheConstants.CATEGORIES_KEY, categories);
            }

            return categories;
        }

        private async Task<Category?> GetSingleCategoryById(int? id)
        {
            var categories = await GetCategoriesUsingCache();
            return categories?.SingleOrDefault(x => x.Id == id);
        }

    }
}
