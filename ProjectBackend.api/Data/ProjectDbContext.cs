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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsDomain>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(200);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CreatedAt).HasColumnType("datetime2");
                entity.Property(p => p.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CategoryDomain>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.CreatedAt).HasColumnType("datetime2");
                entity.Property(c => c.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(c => c.Name).IsUnique();
            });

            modelBuilder.Entity<SupplierDomain>(entity =>
            {
                entity.Property(s => s.Name).HasMaxLength(200);
                entity.Property(s => s.ContactEmail).HasMaxLength(200);
                entity.Property(s => s.Phone).HasMaxLength(50);
                entity.Property(s => s.CreatedAt).HasColumnType("datetime2");
                entity.Property(s => s.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(s => s.Name).IsUnique();
                entity.HasIndex(s => s.ContactEmail)
                    .IsUnique()
                    .HasFilter("[ContactEmail] IS NOT NULL");
            });

            modelBuilder.Entity<CustomerDomain>(entity =>
            {
                entity.Property(c => c.FirstName).HasMaxLength(100);
                entity.Property(c => c.LastName).HasMaxLength(100);
                entity.Property(c => c.Email).HasMaxLength(200);
                entity.Property(c => c.Phone).HasMaxLength(50);
                entity.Property(c => c.CreatedAt).HasColumnType("datetime2");
                entity.Property(c => c.UpdatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<UserDomain>(entity =>
            {
                entity.Property(u => u.Email).HasMaxLength(200);
                entity.Property(u => u.Username).HasMaxLength(100);
                entity.Property(u => u.Password).HasMaxLength(500);
                entity.Property(u => u.Role).HasConversion<string>();
                entity.Property(u => u.CreatedAt).HasColumnType("datetime2");
                entity.Property(u => u.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }

        private void ApplyAuditInformation()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(entity => entity.CreatedAt).IsModified = false;
                    entry.Entity.UpdatedAt = utcNow;
                }
            }
        }
    }
}
