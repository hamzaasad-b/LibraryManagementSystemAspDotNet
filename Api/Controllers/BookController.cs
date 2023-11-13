using System.Drawing;
using Api.Dto.Book;
using Api.Dto.Common;
using Common.Dto.Book;
using Data.Dto;
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
        // [Authorize]
        [HttpGet]
        public async Task<ResponseDto<PaginationDto<BookDto>>> Get([FromQuery] string? term, [FromQuery] int page = 0,
            [FromQuery] int size = 50)
        {
            var result = await _bookService.FindBooksByTerm(page, size, term);
            return Result(result);
        }

        // GET: api/Book/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ResponseDto<BookDto?>> Get(uint id)
        {
            var result = await _bookService.GetById(id);
            return Result(result, true);
        }

        // POST: api/Book
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(ResponseDto))]
        public async Task<ResponseDto<BookDto>> Post([FromBody] CreateBookDto book)
        {
            Console.WriteLine(ModelState.IsValid);
            var newBook = new BookDto()
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
                : BadRequest<BookDto?>();
        }

        [HttpPut("{bookId}/Issue/{userId}")]
        public async Task<ResponseDto> Issue([FromRoute] uint bookId, [FromRoute] uint userId)
        {
            var result = await _bookService.IssueBookToUser(bookId, userId);
            return result.Success
                ? Success()
                : Fail();
        }

        [HttpPut("{bookId}/Return/{userId}")]
        public async Task<ResponseDto> ReturnBookFromUser([FromRoute] uint bookId, [FromRoute] uint userId)
        {
            var result = await _bookService.ReturnBookFromUser(bookId, userId);
            return result.Success
                ? Success()
                : Fail();
        }

        [HttpPut("User/{userId}")]
        public async Task<ResponseDto<PaginationDto<BookDto>>> BooksIssuedToUser([FromRoute] uint userId,
            [FromQuery] int size = 50, [FromQuery] int page = 0)
        {
            var result =
                await _bookService.GetBooksWithPagination(b => b.IssuedToUserId == userId, pageNumber: 0,
                    pageSize: size);
            return Result(result);
        }
    }
}