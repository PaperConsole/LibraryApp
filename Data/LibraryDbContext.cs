using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{

    public class LibraryDbContext : DbContext
    {

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {


        }

        
     public DbSet<Author> Authors { get; set; }   // <--- MUSI być
    public DbSet<Book> Books { get; set; }       // <--- MUSI być
    public DbSet<User> Users { get; set; } 

protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.BorrowedBooks)
                .WithMany(b => b.Borrowers)
                .UsingEntity(j => j.ToTable("UserBooks")); // nazwa tabeli łączącej
        }

    }

}