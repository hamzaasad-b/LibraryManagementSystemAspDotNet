using AutoMapper;
using Common.Dto.Book;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BookRepository : GenericRepository<Book, BookDto>
{
    public BookRepository(LmsDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}