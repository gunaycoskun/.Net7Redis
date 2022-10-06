using Microsoft.EntityFrameworkCore;

namespace NET7RedisAPI.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasData(
                new Product { Id = 1, Name = "Kalem 1", Price = 10 },
                     new Product { Id = 2, Name = "Kalem 2", Price = 11 },
                     new Product { Id = 3, Name = "Kalem 3", Price = 12 }
                );
        }
    }
}
