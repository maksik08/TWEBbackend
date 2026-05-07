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
        public DbSet<OrderDomain> Orders { get; set; }
        public DbSet<OrderItemDomain> OrderItems { get; set; }
        public DbSet<RefreshTokenDomain> RefreshTokens { get; set; }

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
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.Phone).HasMaxLength(50);
                entity.Property(u => u.Balance).HasColumnType("decimal(18,2)");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<OrderDomain>(entity =>
            {
                entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.CreatedAt).HasColumnType("datetime2");
                entity.Property(o => o.UpdatedAt).HasColumnType("datetime2");
                entity.Property(o => o.PaidAt).HasColumnType("datetime2");

                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(o => o.UserId);
                entity.HasIndex(o => o.Status);
            });

            modelBuilder.Entity<OrderItemDomain>(entity =>
            {
                entity.Property(i => i.ProductName).HasMaxLength(200);
                entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(i => i.Product)
                    .WithMany()
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshTokenDomain>(entity =>
            {
                entity.Property(t => t.TokenHash).HasMaxLength(128).IsRequired();
                entity.Property(t => t.CreatedAt).HasColumnType("datetime2");
                entity.Property(t => t.ExpiresAt).HasColumnType("datetime2");
                entity.Property(t => t.RevokedAt).HasColumnType("datetime2");
                entity.Ignore(t => t.IsActive);

                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.TokenHash).IsUnique();
                entity.HasIndex(t => t.UserId);
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
