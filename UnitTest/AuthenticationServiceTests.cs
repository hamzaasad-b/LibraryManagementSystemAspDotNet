using Data.Context;
using Data.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class AuthenticationServiceTests
{
    private AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        var options = new DbContextOptionsBuilder<LmsDbContext>()
            .UseSqlite(@"Data Source=C:/testDbs/auth.db")
            .Options;
        var dbContext = new LmsDbContext(options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        var userStore = new UserStore<User, IdentityRole<uint>, LmsDbContext, uint>(dbContext);
        _authenticationService = new AuthenticationService(
            new UserManager<User>(userStore, null, null, null, null, null, null, null, null)
        );
    }

    [Fact]
    public async Task CreateUser()
    {
        var result = await _authenticationService.RegisterUser(email: "hamzaasad@folio3.com", "Folio3Random123!@#");
        Assert.True(result.Success);
    }
}