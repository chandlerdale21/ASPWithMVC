using BooksReadTrackerDatabaseLayer;
using BooksReadTrackerDBLibrary;
using BooksReadTrackerModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;
namespace TestBooksReadTracker;


public class TestBooksRepository
{
    private BooksRepository _repo;
    private DbContextOptions<BooksReadTrackerDbContext> _options;
    private const string USER_ID = "e9ae082f-5a20-46fd-ba54-5f388e4eb918";
    private const string USER_ID2 = "e9ae082g-5a21-46fe-ba55-5f388e4eb919";
    private const string USER_ID3 = "c3f38696-64bf-4021-8dba-11ae53de0afc";
    private const string USER_ID4 = "c3f38697-64bg-4022-8dbb-11ae53de0afd";

    public TestBooksRepository()
    {
        SetupOptions();
        SeedData();
    }

    private void SetupOptions()
    {
        _options = new DbContextOptionsBuilder<BooksReadTrackerDbContext>()
                    .UseInMemoryDatabase(databaseName: "ItemsWebManagerDbTests")
                    .Options;
    }

    private List<Category> Categories()
    {
        //create at least two categories
        return new List<Category>(){
            new Category() { Id = 1, Name = "Food" },
            new Category() { Id = 2, Name = "Hotel" }
        };
    }

    private List<Book> Items()
    {
        //create 5 items
        //3 for USER_ID 1
        //2 for USER_ID 2 
        //spread the categories around
        return new List<Book>() {
            new Book() { Id = 1, CategoryId = 1, FilePath = "Some Path", Name = "Hamburger", UserId = USER_ID, Notes = "Some notes", PagesRead = "50", TotalPages = "100"  },
            new Book() { Id = 2, CategoryId = 1, FilePath = "Some Path", Name = "Pizza", UserId = USER_ID, Notes = "Some notes", PagesRead = "50", TotalPages = "10"  },
            new Book() { Id = 3, CategoryId = 2, FilePath = "Some Path", Name = "Holiday Inn", UserId = USER_ID2 , Notes = "Some notes", PagesRead = "50", TotalPages = "100" },
            new Book() { Id = 4, CategoryId = 2, FilePath = "Some Path", Name = "La Quinta Inn", UserId = USER_ID , Notes = "Some notes", PagesRead ="50", TotalPages = "100" },
            new Book() { Id = 5, CategoryId = 1, FilePath = "Some Path", Name = "Taco", UserId = USER_ID2, Notes = "Some notes", PagesRead = "50", TotalPages = "100"  }
        };
    }

    private void SeedData()
    {
        var cats = Categories();
        var items = Items();

        using (var context = new BooksReadTrackerDbContext(_options))
        {
            var existingCategories = Task.Run(() => context.Categories.ToListAsync()).Result;
            if (existingCategories is null || existingCategories.Count == 0)
            {
                context.Categories.AddRange(cats);
                context.SaveChanges();
            }

            var existingItems = Task.Run(() => context.Books.ToListAsync()).Result;
            if (existingItems is null || existingItems.Count == 0)
            {
                context.Books.AddRange(items);
                context.SaveChanges();
            }
        }

    }

    [Theory]
    [InlineData(USER_ID, 3)]
    [InlineData(USER_ID2, 2)]
    [InlineData(USER_ID3, 0)]
    public async Task TestGetAllItemsGood(string userId, int expectedCount)
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var items = await _repo.GetAllAsync(userId);

            //Assert
            Assert.NotNull(items);
            items.ShouldNotBeNull();

            items.Count.ShouldBe(expectedCount, "Book count did not match expected for each user");
        }
    }

    [Theory]
    [InlineData(1, "Hamburger", 1, USER_ID)]
    [InlineData(2, "Pizza", 1, USER_ID)]
    [InlineData(3, "Holiday Inn", 2, USER_ID2)]
    [InlineData(4, "La Quinta Inn", 2, USER_ID)]
    [InlineData(5, "Taco", 1, USER_ID2)]
    public async Task TestGetAsyncGood(int id, string name, int categoryId, string userId)
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var Book = await _repo.GetAsync(id, userId);

            //Assert
            Book.ShouldNotBeNull();
            Book.Id.ShouldBe(id);
            Book.Name.ShouldBe(name);
            Book.CategoryId.ShouldBe(categoryId);
        }
    }

    [Theory]
    [InlineData(3, USER_ID)]
    [InlineData(4, USER_ID2)]
    public async Task TestGetAsyncBadValidIdWrongUser(int id, string userId)
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var Book = await _repo.GetAsync(id, userId);
            Book.ShouldBe(null);
        }
    }

    [Fact]
    public async Task TestGetAsyncBadInvalidId()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var Book = await _repo.GetAsync(-1, USER_ID);

            //Assert
            Assert.Null(Book);
            Book.ShouldBeNull();
        }
    }

    [Fact]
    public async Task TestGetAsyncBadValidIdNoRecord()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var Book = await _repo.GetAsync(55, USER_ID);

            //Assert
            Assert.Null(Book);
            Book.ShouldBeNull();
        }
    }

    //test add good
    [Fact]
    public async Task TestAddAsync()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID4, Notes = "Some notes", PagesRead = "50", TotalPages = "100" };

            //Act
            var result = await _repo.AddOrUpdateAsync(Book, USER_ID4);

            //Assert
            result.ShouldBeGreaterThan(0);
            Book.Id.ShouldBeGreaterThan(0);

            //ensure it is there:
            var itemCheck = await _repo.GetAsync(Book.Id, USER_ID4);
            itemCheck.ShouldNotBeNull();
            itemCheck.CategoryId.ShouldBe(2);
            itemCheck.UserId.ShouldBe(USER_ID4);
            itemCheck.Name.ShouldBe("Marriot");
            itemCheck.FilePath.ShouldBe("Some Path");

            //remove the Book
            await _repo.DeleteAsync(Book.Id, USER_ID4);
        }
    }

    [Fact]
    public async Task TestAddAsyncBadUserMismatch()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID4 };

            //Act
            Should.Throw<ArgumentException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestAddAsyncBadNullItem()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            Book Book = null;

            //Act
            Should.Throw<ArgumentException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestAddAsyncBadItemUserIDEmpty()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = string.Empty };

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestAddAsyncBadItemUserIDNull()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = null };

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestUpdateGood()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = await _repo.GetAsync(3, USER_ID2);

            //Act
            Book.Name = "Red Roof Inn";
            Book.CategoryId = 1;
            Book.FilePath = "This is the way";

            var result = await _repo.AddOrUpdateAsync(Book, USER_ID2);
            result.ShouldBe(Book.Id);

            var itemCheck = await _repo.GetAsync(Book.Id, USER_ID2);
            itemCheck.Name.ShouldBe("Red Roof Inn");
            itemCheck.CategoryId.ShouldBe(1);
            itemCheck.FilePath.ShouldBe("This is the way");

            //reset the Book
            var resetItem = new Book() { Id = 3, CategoryId = 2, FilePath = "Some Path", Name = "Holiday Inn", UserId = USER_ID2 };
            await _repo.AddOrUpdateAsync(resetItem, USER_ID2);
        }
    }

    [Fact]
    public async Task TestUpdateBadNoMatchingRecord()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 70, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID };

            //Act
            Should.Throw<Exception>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestAddOrUpdateBadInvalidId()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = -1, CategoryId = 2, FilePath = "Some Path", Name = "Marriot" };

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    //TODO: tests for add/update bad invalid category id
    [Fact]
    public async Task TestAddOrUpdateBadInvalidCategoryIdOutsideMin()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = -1, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID };

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestAddOrUpdateBadInvalidCategoryIdOutsideMax()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 500, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID };

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.AddOrUpdateAsync(Book, USER_ID));
        }
    }

    //test delete 1 good
    [Fact]
    public async Task TestDeleteAsyncByItemGood()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID4, Notes = "Some notes", PagesRead = "50", TotalPages = "100" };
            var result = await _repo.AddOrUpdateAsync(Book, USER_ID4);

            //Act
            var deleteResult = await _repo.DeleteAsync(Book, USER_ID4);

            //Assert
            deleteResult.ShouldBe(Book.Id);

            var itemCheck = await _repo.GetAsync(Book.Id, USER_ID4);
            itemCheck.ShouldBeNull();
        }
    }

    //test delete 1 bad null Book
    [Fact]
    public async Task TestDeleteAsyncByItemBadNullItem()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            Book Book = null;

            //Act
            Should.Throw<ArgumentException>(async () => await _repo.DeleteAsync(Book, USER_ID));
        }
    }

    [Fact]
    public async Task TestDeleteAsyncByItemBadUserIdMismatch()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = await _repo.GetAsync(3, USER_ID2);
            Book.ShouldNotBeNull();
            Book.UserId.ShouldBe(USER_ID2);

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.DeleteAsync(Book, USER_ID));
        }
    }

    //test delete 2 good    
    [Fact]
    public async Task TestDeleteAsyncByIdGood()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);
            var Book = new Book() { Id = 0, CategoryId = 2, FilePath = "Some Path", Name = "Marriot", UserId = USER_ID4, Notes = "Some notes", PagesRead = "50", TotalPages = "100" };
            var result = await _repo.AddOrUpdateAsync(Book, USER_ID4);

            //ensure it is there:
            var itemCheck = await _repo.GetAsync(Book.Id, USER_ID4);
            itemCheck.ShouldNotBeNull();

            //Act
            var deleteResult = await _repo.DeleteAsync(Book.Id, USER_ID4);

            //Assert
            deleteResult.ShouldBe(Book.Id);

            itemCheck = await _repo.GetAsync(Book.Id, USER_ID4);
            itemCheck.ShouldBeNull();
        }
    }

    //test delete 2 bad id < 1
    [Fact]
    public async Task TestDeleteAsyncByIdBadIdLessThan1()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.DeleteAsync(-5, USER_ID));
        }
    }

    //test delete 2 bad > 0 no matching record
    [Fact]
    public async Task TestDeleteAsyncByIdBadNoMatchingRecord()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.DeleteAsync(100, USER_ID));
        }
    }

    //test delete 2 bad > 0 no matching record
    [Fact]
    public async Task TestDeleteAsyncByIdBadUserMismatch()
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            Should.Throw<ArgumentNullException>(async () => await _repo.DeleteAsync(1, USER_ID3));
        }
    }

    //test exists good
    //test exists bad
    [Theory]
    [InlineData(1, USER_ID, true)]
    [InlineData(2, USER_ID, true)]
    [InlineData(3, USER_ID, false)]
    [InlineData(1, USER_ID2, false)]
    [InlineData(3, USER_ID2, true)]
    [InlineData(50, USER_ID, false)]
    public async Task TestExistsAsync(int id, string userId, bool expected)
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            var result = await _repo.ExistsAsync(id, userId);

            //Assert
            result.ShouldBe(expected);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task TestExistsAsyncBadIdLessThan1(int id)
    {
        using (var context = new BooksReadTrackerDbContext(_options))
        {
            //Arrange
            _repo = new BooksRepository(context);

            //Act
            Should.Throw<ArgumentOutOfRangeException>(async () => await _repo.ExistsAsync(id, USER_ID));
        }
    }
}




