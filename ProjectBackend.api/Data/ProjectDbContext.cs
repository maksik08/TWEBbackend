using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<ProductsDomain> Products { get; set; }
        public DbSet<CategoryDomain> Categories { get; set; }
        public DbSet<SupplierDomain> Suppliers { get; set; }
        public DbSet<CustomerDomain> Customers { get; set; }
        public DbSet<UserDomain> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDomain>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}
