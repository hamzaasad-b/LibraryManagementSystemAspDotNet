using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BookRepository : GenericRepository<Book>
{
    public BookRepository(DbContext context) : base(context)
    {
    }
}