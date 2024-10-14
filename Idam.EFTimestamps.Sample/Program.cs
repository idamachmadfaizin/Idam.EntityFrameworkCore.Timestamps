using Idam.EFTimestamps.Sample.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
if (!builder.Environment.IsProduction()) builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDbContext>(options => { options.UseSqlite("SampleDatabase"); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/api/ExceptionHandler/HandleError");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/api/ExceptionHandler/HandleErrorDevelopment");
}

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();