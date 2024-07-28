using DecorStore.BL.Models;
using Microsoft.EntityFrameworkCore;

namespace DecorStore.Data.Shared
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Section> Sections { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("Sections_tb", "Product.Category");
                entity.HasMany(s => s.Categories)
                    .WithOne(c => c.Section)
                    .HasForeignKey(c => c.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories_tb", "Product.Category");
                entity.HasMany(c => c.Subcategories)
                    .WithOne(s => s.Category)
                    .HasForeignKey(s => s.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subcategory>().ToTable("Subcategories_tb", "Product.Category");
        }
    }
}
