using System.Linq.Expressions;
using Common.Dto.Book;
using Data.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace UnitTest;

public class BookRepositoryTests : RepositoryTests, IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BookRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Add_BookToDatabase()
    {
        var book = new BookDto { Iban = "1234567890", Title = "Sample Book" };

        var addedBook = await _repository.Add(book);

        Assert.NotNull(addedBook);
        Assert.Equal("1234567890", addedBook.Iban);
        Assert.Equal("Sample Book", addedBook.Title);
    }

    [Fact]
    public async Task Update_BookInDatabase()
    {
        var book = new BookDto { Iban = "1234567890", Title = "Sample Book" };

        var addedBook = await _repository.Add(book);

        Assert.NotNull(addedBook);
        Assert.Equal("1234567890", addedBook.Iban);
        Assert.Equal("Sample Book", addedBook.Title);
        if (addedBook.Id.HasValue is false) throw new Exception();
        var updateDto = new BookDto() { Iban = "12312jkh123", Title = "Updated Title"};
        var updatedBook = await _repository.Update(addedBook.Id.Value, updateDto);
        Assert.Equal("12312jkh123", updatedBook.Iban);
        Assert.Equal("Sample Book", updatedBook.Title);
    }


    [Fact]
    public async Task GetById_ReturnsBook()
    {
        var book = new BookDto { Iban = "1234567890", Title = "Sample Book" };
        var entity = await _repository.Add(book);
        if (entity.Id.HasValue is false) throw new Exception();
        var retrievedBook = await _repository.GetById(entity.Id.Value);

        Assert.NotNull(retrievedBook);
        Assert.Equal("1234567890", retrievedBook.Iban);
        Assert.Equal("Sample Book", retrievedBook.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNullForInvalidId()
    {
        var retrievedBook = await _repository.GetById(999); // Assuming this ID does not exist.

        Assert.Null(retrievedBook);
    }

    [Fact]
    public async Task Delete_BookFromDatabase()
    {
        var book = new BookDto { Iban = "1234567890", Title = "Sample Book" };
        await _repository.Add(book);
        if (book.Id.HasValue is false) throw new Exception();
        var result = await _repository.Delete(book.Id.Value);

        Assert.True(result);
        Assert.Empty(_dbContext.Set<Book>());
    }


    [Fact]
    public async Task AddMultiple_AddsMultipleBooks()
    {
        // Create a list of books to add
        var booksToAdd = new List<BookDto>
        {
            new BookDto { Iban = "123", Title = "Book 1" },
            new BookDto { Iban = "456", Title = "Book 2" },
            new BookDto { Iban = "789", Title = "Book 3" },
            new BookDto { Iban = "111", Title = "Book 4" },
            new BookDto { Iban = "222", Title = "Book 5" },
            new BookDto { Iban = "333", Title = "Book 6" },
        };

        // Add the books using AddMultiple
        var addedBooks = await _repository.AddMultiple(booksToAdd);

        // Check if the books were added and the returned list matches the input
        Assert.Equal(6, addedBooks.Count());
        Assert.All(addedBooks, book => Assert.True(booksToAdd.Contains(book)));
    }

    [Fact]
    public async Task GetFilteredWithPagination_ReturnsPagedResultWithFilter()
    {
        // Add some sample books to the database
        var books = new[]
        {
            new Book { Iban = "123", Title = "Book 1" },
            new Book { Iban = "496", Title = "Book 2" },
            new Book { Iban = "789", Title = "Book 3" },
            new Book { Iban = "111", Title = "Book 4" },
            new Book { Iban = "222", Title = "Book 5" },
            new Book { Iban = "333", Title = "Book 6" },
        };
        _dbContext.Set<Book>().AddRange(books);
        await _dbContext.SaveChangesAsync();

        // Create a filter to get books with '2' in their Iban
        var term = "2";
        // Expression<Func<Book, bool>> filter = book => book.Iban.Contains("2");
        Expression<Func<Book, bool>> titleSearch = b => b.Title.Contains(term);
        Expression<Func<Book, bool>> ibanSearch = b => b.Iban.Contains(term);

        // Create a parameter expression
        var parameter = Expression.Parameter(typeof(Book), "b");

        var titleSearchBody = Expression.Invoke(titleSearch, parameter);
        var ibanSearchBody = Expression.Invoke(ibanSearch, parameter);

        var combinedCondition = Expression.OrElse(titleSearchBody, ibanSearchBody);

        var filter = Expression.Lambda<Func<Book, bool>>(combinedCondition, parameter);

        // Get the first page of filtered books (2 items per page)
        var result = await _repository.GetFilteredWithPagination(filter, pageSize: 2, pageNumber: 1);

        // Check the result
        Assert.Equal(2, result.Data.ToList().Count);
        Assert.Equal(3, result.TotalRecords);

        // Ensure the correct books are in the first page
        Assert.Equal("123", result.Data.First().Iban);
        Assert.Equal("496", result.Data.Last().Iban);
    }

    [Fact]
    public async Task GetAllWithPagination_ReturnsPagedResultWithoutFilter()
    {
        // Add some sample books to the database
        var books = new[]
        {
            new Book { Iban = "123", Title = "Book 1" },
            new Book { Iban = "456", Title = "Book 2" },
            new Book { Iban = "789", Title = "Book 3" },
            new Book { Iban = "111", Title = "Book 4" },
            new Book { Iban = "222", Title = "Book 5" },
            new Book { Iban = "333", Title = "Book 6" },
        };
        _dbContext.Set<Book>().AddRange(books);
        await _dbContext.SaveChangesAsync();

        // Get the first page (2 items per page) without a filter
        var result = await _repository.GetAllWithPagination(pageSize: 2, pageNumber: 1);

        // Check the result
        Assert.Equal(2, result.Data.ToList().Count);
        Assert.Equal(6, result.TotalRecords);

        // Ensure the correct books are in the first page
        Assert.Equal("123", result.Data.First().Iban);
        Assert.Equal("456", result.Data.Last().Iban);

        var result2 = await _repository.GetAllWithPagination(pageSize: 2, pageNumber: 2);

        Assert.Equal("789", result2.Data.First().Iban);
        Assert.Equal("111", result2.Data.Last().Iban);
    }


    // Add more test methods to cover other functionalities.

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}