//using BooksReadTrackerDatabaseLayer;
//using BooksReadTrackerDBLibrary;
//using BooksReadTrackerModels;
//using Microsoft.EntityFrameworkCore;
//using Shouldly;
//namespace TestBooksReadTracker;


//public class TestBooksRepository
//{
//    private BooksRepository _repo;

//    private DbContextOptions<BooksReadTrackerDbContext> _options;

//    public TestBooksRepository()
//    {
//        SetupOptions();
//        SeedData();
//    }
//    private void SetupOptions()
//    {
//        _options = new DbContextOptionsBuilder<BooksReadTrackerDbContext>()
//            .UseInMemoryDatabase(databaseName: "BooksReadTrackerTests").Options;
//    }

//    private List<Category> Categories()
//    {
//        return new List<Category>()
//        {
//            new Category() {Id = 1, Name = "Food" },
//            new Category() {Id = 2, Name = "Hotel" }
//        };

//    }
//    private List<Book> Books()
//    {
//        //create at least three items
//        return new List<Book>() {
//            new Book() { Id = 1, CategoryId = 1, FilePath = "Some Path", Name = "Percy Jackson" },
//            new Book() { Id = 2, CategoryId = 1, FilePath = "Some Path", Name = "LOTR" },
//            new Book() { Id = 3, CategoryId = 2, FilePath = "Some Path", Name = "RandomBook" }
//        };
//    }
//    private void SeedData()
//    {
//        var cats = Categories();
//        var books = Books();

//        using (var context = new BooksReadTrackerDbContext(_options))
//        {
//            var existingCategories = Task.Run(() => context.Categories.ToListAsync()).Result;
//            if (existingCategories is null || existingCategories.Count == 0)
//            {
//                context.Categories.AddRange(cats);
//                context.SaveChanges();
//            }

//            var existingItems = Task.Run(() => context.Books.ToListAsync()).Result;
//            if (existingItems is null || existingItems.Count == 0)
//            {
//                context.Books.AddRange(books);
//                context.SaveChanges();
//            }
//        }
//    }

//    [Fact]

//    public async Task GetAllItems()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);
//            var items = await _repo.GetAllAsync();

//            Assert.NotNull(Books);
//            items.ShouldNotBeNull();

//            items.Count.ShouldBe(3, "You didn't make the items or something...");


//        }

//    }

//    [Fact]

//    public async Task TestAddBookItme()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);
//            var testBook = new Book() { Id = 0, Name = "Harry Potter", CategoryId = 1, FilePath = "teshalj" };


//            var result = await _repo.AddOrUpdateAsync(testBook);

//            result.ShouldBeGreaterThan(0);
//            testBook.Id.ShouldBeGreaterThan(0);

//            await _repo.DeleteAsync(testBook.Id);
//        }

//    }

//    [Fact]

//    public async Task TestAddBookItemBadInvalidId()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);
//            Book testBook = null;


//            Assert.ThrowsAsync<ArgumentException>(async () => await _repo.AddOrUpdateAsync(testBook));

//        }
//    }

//    [Theory]
//    [InlineData(1, "Percy Jackson", 1)]
//    [InlineData(2, "LOTR", 1)]
//    [InlineData(3, "RandomBook", 2)]

//    public async Task TestGetAsync(int id, string name, int categoryId)
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {
//            _repo = new BooksRepository(context);

//            var bookItem = await _repo.GetAsync(id);

//            bookItem.ShouldNotBeNull();
//            bookItem.Id.ShouldBe(id);
//            bookItem.CategoryId.ShouldBe(categoryId);
//        }

//    }
//    [Fact]
//    public async Task TestGetAsyncBadValidIdNoRecord()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);
//            var testBook = await _repo.GetAsync(36);


//            Assert.ThrowsAsync<ArgumentException>(async () => await _repo.AddOrUpdateAsync(testBook));

//        }
//    }

//    [Fact]
//    public async Task TestUpdate()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);

//            var testBook = new Book() { Id = 3, CategoryId = 2, FilePath = "Some Path", Name = "The Bible" };

//            var result = await _repo.AddOrUpdateAsync(testBook);

//            result.ShouldBe(testBook.Id);

//            var bookCheck = await _repo.GetAsync(testBook.Id);

//            bookCheck.CategoryId.ShouldBe(testBook.CategoryId);
//            bookCheck.Id.ShouldBe(testBook.Id);



//        }
//    }
//    [Fact]
//    public async Task TestUpdateBad()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);

//            var testBook = new Book() { Id = 70, CategoryId = 2, FilePath = "Some Path", Name = "The Bible" };

//            Assert.ThrowsAsync<ArgumentException>(async () => await _repo.AddOrUpdateAsync(testBook));

//        }
//    }

//    [Fact]

//    public async Task TestAddOrUpdtedBadInvalidCategoryId()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {

//            _repo = new BooksRepository(context);

//            var testBook = new Book() { Id = 0, CategoryId = -7, FilePath = "Some Path", Name = "Money" };

//            Should.ThrowAsync<DivideByZeroException>(async () => await _repo.AddOrUpdateAsync(testBook));

//        }
//    }
//    [Fact]
//    public async Task TestDeleteAsyncGoodItem()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {
//            _repo = new BooksRepository(context);
//            var testBook = new Book() { Id = 0, Name = "Harry Potter", CategoryId = 1, FilePath = "teshalj" };


//            var result = await _repo.AddOrUpdateAsync(testBook);

//            result.ShouldBeGreaterThan(0);
//            testBook.Id.ShouldBeGreaterThan(0);

//            result = await _repo.DeleteAsync(testBook.Id);

//            result.ShouldBe(testBook.Id);

//            var existing = await _repo.GetAsync(testBook.Id);
//            existing.ShouldBeNull();

//        }
//    }
//    [Fact]
//    public async Task TestDeleteAsyncBadItem()
//    {
//        using (var context = new BooksReadTrackerDbContext(_options))
//        {



//        }
//    }


//}




