using System.Runtime.CompilerServices;
using Data.Context;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class RepositoryTests
{
    protected readonly LmsDbContext _dbContext;
    protected readonly BookRepository _repository;

    public RepositoryTests(string connectionString = @"Data Source=C:\testDbs\test.db")
    {
        DbContextOptions<LmsDbContext> options = new DbContextOptionsBuilder<LmsDbContext>()
            .UseSqlite(
                connectionString
            ).Options;

        _dbContext = new LmsDbContext(options);
        _repository = new BookRepository(_dbContext);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }
}