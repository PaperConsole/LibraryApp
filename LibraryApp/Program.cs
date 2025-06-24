using LibraryApp.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    if (!context.Users.Any())
    {
        var jsonData = File.ReadAllText("seedData.json");
        var seedData = JsonSerializer.Deserialize<SeedData>(jsonData);

        // 1. Najpierw dodaj autorów i użytkowników
        context.Authors.AddRange(seedData.Authors);
        context.Users.AddRange(seedData.Users);
        context.SaveChanges();

        // 2. Dodaj książki, ustawiając autora na podstawie Id
        foreach (var book in seedData.Books)
{
    context.Books.Add(new Book
    {
        Title = book.Title,
        PublishedYear = book.PublishedYear,
        IsAvailable = book.IsAvailable,
        AuthorId = book.AuthorId
    });
}

        context.SaveChanges();

        // 3. Ustaw relacje wiele-do-wielu (wypożyczenia)
   foreach (var user in seedData.Users)
{
    var dbUser = context.Users.Include(u => u.BorrowedBooks).FirstOrDefault(u => u.Username == user.Username);
    if (dbUser == null) continue;

    var booksToAdd = new List<Book>();

    foreach (var book in user.BorrowedBooks)
    {
        var dbBook = context.Books.FirstOrDefault(b => b.Title == book.Title);
        if (dbBook != null)
        {
            booksToAdd.Add(dbBook);
        }
    }

    dbUser.BorrowedBooks ??= new List<Book>();
    foreach (var b in booksToAdd)
    {
        dbUser.BorrowedBooks.Add(b);
    }
}

        context.SaveChanges();
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();

public class SeedData
{
    public List<Author> Authors { get; set; }
    public List<Book> Books { get; set; }
    public List<User> Users { get; set; }
}