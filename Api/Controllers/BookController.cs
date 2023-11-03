using Api.Dto.Book;
using Api.Dto.Common;
using Data.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : BaseController
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ResponseDto<IEnumerable<Book>>> Get([FromQuery] string? term)
        {
            var result = await _bookService.FindBooksByTerm(term);
            return !result.Success
                ? Fail<IEnumerable<Book>>(default)
                : Success(result.Data);
        }

        // GET: api/Book/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ResponseDto<Book?>> Get(uint id)
        {
            var result = await _bookService.GetById(id);
            if (!result.Success)
            {
                return Fail<Book?>(default);
            }

            return Result(result, true);
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ResponseDto<Book>> Post([FromBody] CreateBookDto book)
        {
            var newBook = new Book()
            {
                Iban = book.Iban,
                Title = book.Title
            };
            var result = await _bookService.AddBook(newBook);
            return Result(result);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<ResponseDto> Put(uint id, [FromBody] CreateBookDto book)
        {
            var res = await _bookService.GetById(id);
            if (!res.Success || res.Data is null)
            {
                return Fail();
            }

            res.Data.Iban = book.Iban;
            res.Data.Title = book.Title;

            var result = await _bookService.Update(id, res.Data);
            return Result(result);
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(uint id)
        {
            var result = await _bookService.DeleteTEntity(id);
            return result.Success
                ? Success()
                : BadRequest<Book?>();
        }

        [HttpPut("IssueBookToUser")]
        public async Task<ResponseDto> IssueBookToUser([FromBody] IssueBookToUserDto body)
        {
            var result = await _bookService.IssueBookToUser(bookId: body.BookId, userId: body.UserId);
            return result.Success
                ? Success()
                : Fail();
        }

        [HttpPut("ReturnBookFromUser")]
        public async Task<ResponseDto> ReturnBookFromUser([FromBody] IssueBookToUserDto body)
        {
            var result = await _bookService.ReturnBookFromUser(bookId: body.BookId, userId: body.UserId);
            return result.Success
                ? Success()
                : Fail();
        }
    }
}