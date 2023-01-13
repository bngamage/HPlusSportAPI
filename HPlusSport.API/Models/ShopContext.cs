using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Models
{
    // entity framework (Object Relational mapper) communicates with DB and we dont have to write sql queries
    public class ShopContext : DbContext

    {

        public ShopContext(DbContextOptions<ShopContext> options): base(options) {

        }

        // runs ones the model is created
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // define how products and cateogories are related
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId);

            // populate sample data
            modelBuilder.Seed();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
