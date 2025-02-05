using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Data
{
    public class LibraryManagementDbContext : DbContext
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
            : base(options) { }

        public DbSet<AuthorEntity> Authors => Set<AuthorEntity>();
        public DbSet<BookEntity> Books => Set<BookEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<BookOrderEntity> BookOrders => Set<BookOrderEntity>();
        public DbSet<GenreEntity> Genres => Set<GenreEntity>();
        public DbSet<BookGenreEntity> BookGenres => Set<BookGenreEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<OrderEntity>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BookEntity>().Property(b => b.Price).HasColumnType("decimal(18,2)");
        }
    }
}
