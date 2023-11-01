using Api.Middleware;
using Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LmsDbContext>( options => options.UseSqlite("DataSource=app.db") );

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