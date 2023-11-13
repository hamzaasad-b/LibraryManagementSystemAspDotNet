using System.Runtime.CompilerServices;
using AutoMapper;
using Common.Dto.Book;
using Common.Dto.User;
using Data;
using Data.Context;
using Data.Entities;
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

        var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<AutoMapperProfile>(); }));
        _dbContext = new LmsDbContext(options);
        _repository = new BookRepository(_dbContext, mapper);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }
}