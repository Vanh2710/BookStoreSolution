using BookStore.Models.Entities;
using Microsoft.EntityFrameworkCore;
//
namespace BookStore.Models.Data
{
  public class AppDbContext : DbContext
  {
    //1. Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    //2. Khai bao cac bang
    public DbSet<Book> Books {get; set;}
    public DbSet<User> Users {get; set;}

    //3. Them logic cho cac bang
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();
    }
  }
}