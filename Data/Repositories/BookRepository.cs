using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BookRepository : GenericRepository<Book>
{
    public BookRepository(LmsDbContext context) : base(context)
    {
    }
}