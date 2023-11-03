using System.Reflection;
using Api.Middleware;
using Api.Validators;
using Data.Context;
using Data.Entities;
using Data.Repositories;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Add services to the container.
builder.Services.AddDbContext<LmsDbContext>(options => options.UseSqlite(config["ConnectionStrings:Default"]));

builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<UserRepository>();

// Add ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole<uint>>()
    .AddEntityFrameworkStores<LmsDbContext>()
    .AddDefaultTokenProviders();

// Register the UserManager
builder.Services.AddScoped<UserManager<User>>();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenticationService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

app.UseSwagger();
app.UseSwaggerUI();

//}

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
        await GenericApiErrorHandler.HandleErrorAsync(context, null, app.Environment.IsDevelopment()));
});
app.UseAuthorization();

app.MapControllers();

app.Run();