using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess
{
    public class ProjectDbContext : DbContext
    {
        private static readonly JsonSerializerOptions JsonOptions = new();

        private static readonly ValueConverter<List<string>, string> StringListConverter = new(
            v => JsonSerializer.Serialize(v ?? new List<string>(), JsonOptions),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>());

        private static readonly ValueComparer<List<string>> StringListComparer = new(
            (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
            v => v == null ? 0 : v.Aggregate(0, (h, s) => HashCode.Combine(h, s == null ? 0 : s.GetHashCode())),
            v => v == null ? new List<string>() : v.ToList());

        private static readonly ValueConverter<List<ProductSpecification>, string> SpecListConverter = new(
            v => JsonSerializer.Serialize(v ?? new List<ProductSpecification>(), JsonOptions),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<ProductSpecification>()
                : JsonSerializer.Deserialize<List<ProductSpecification>>(v, JsonOptions) ?? new List<ProductSpecification>());

        private static readonly ValueComparer<List<ProductSpecification>> SpecListComparer = new(
            (a, b) => JsonSerializer.Serialize(a ?? new(), JsonOptions) == JsonSerializer.Serialize(b ?? new(), JsonOptions),
            v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
            v => v == null
                ? new List<ProductSpecification>()
                : v.Select(s => new ProductSpecification { Label = s.Label, Value = s.Value }).ToList());

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<ProductsDomain> Products { get; set; }
        public DbSet<CategoryDomain> Categories { get; set; }
        public DbSet<SupplierDomain> Suppliers { get; set; }
        public DbSet<CustomerDomain> Customers { get; set; }
        public DbSet<UserDomain> Users { get; set; }
        public DbSet<OrderDomain> Orders { get; set; }
        public DbSet<OrderItemDomain> OrderItems { get; set; }
        public DbSet<RefreshTokenDomain> RefreshTokens { get; set; }
        public DbSet<PasswordResetTokenDomain> PasswordResetTokens { get; set; }
        public DbSet<ContactMessageDomain> ContactMessages { get; set; }
        public DbSet<ServiceRequestDomain> ServiceRequests { get; set; }
        public DbSet<ServiceRequestCommentDomain> ServiceRequestComments { get; set; }
        public DbSet<WorkPhotoDomain> WorkPhotos { get; set; }
        public DbSet<NotificationDomain> Notifications { get; set; }
        public DbSet<ActionLogDomain> ActionLogs { get; set; }
        public DbSet<PaymentTransactionDomain> PaymentTransactions { get; set; }
        public DbSet<IdempotencyRecordDomain> IdempotencyRecords { get; set; }
        public DbSet<ProductReviewDomain> ProductReviews { get; set; }
        public DbSet<ReturnDomain> Returns { get; set; }
        public DbSet<CouponDomain> Coupons { get; set; }
        public DbSet<ServiceTariffDomain> ServiceTariffs { get; set; }
        public DbSet<SupportTicketDomain> SupportTickets { get; set; }
        public DbSet<SupportMessageDomain> SupportMessages { get; set; }
        public DbSet<SupportAttachmentDomain> SupportAttachments { get; set; }
        public DbSet<KnowledgeBaseArticleDomain> KnowledgeBaseArticles { get; set; }
        public DbSet<WarrantyClaimDomain> WarrantyClaims { get; set; }
        public DbSet<RemoteDiagnosticSessionDomain> RemoteDiagnosticSessions { get; set; }
        public DbSet<WarehouseDomain> Warehouses { get; set; }
        public DbSet<WarehouseZoneDomain> WarehouseZones { get; set; }
        public DbSet<ProductStockReservationDomain> ProductStockReservations { get; set; }
        public DbSet<StockMovementDomain> StockMovements { get; set; }
        public DbSet<InventoryCountDomain> InventoryCounts { get; set; }
        public DbSet<InventoryCountItemDomain> InventoryCountItems { get; set; }
        public DbSet<PurchaseOrderDomain> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItemDomain> PurchaseOrderItems { get; set; }
        public DbSet<GoodsReceiptDomain> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItemDomain> GoodsReceiptItems { get; set; }
        public DbSet<SupplierReturnDomain> SupplierReturns { get; set; }
        public DbSet<SupplierReturnItemDomain> SupplierReturnItems { get; set; }
        public DbSet<WarehouseTransferDomain> WarehouseTransfers { get; set; }
        public DbSet<WarehouseDocumentDomain> WarehouseDocuments { get; set; }
        public DbSet<ProductReviewPhotoDomain> ProductReviewPhotos { get; set; }
        public DbSet<ProductReviewReportDomain> ProductReviewReports { get; set; }
        public DbSet<ReviewResponseTemplateDomain> ReviewResponseTemplates { get; set; }

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
                entity.Property(p => p.StockQuantity).HasDefaultValue(0);
                entity.Property(p => p.ReservedQuantity).HasDefaultValue(0);
                entity.Property(p => p.MinStockLevel).HasDefaultValue(0);
                entity.Property(p => p.IsPreorder).HasDefaultValue(false);
                entity.Property(p => p.IsVisible).HasDefaultValue(true);
                entity.Property(p => p.Availability)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .HasDefaultValue(ProductAvailability.InStock);
                entity.Property(p => p.Technology)
                    .HasConversion(StringListConverter, StringListComparer)
                    .HasColumnType("nvarchar(max)");
                entity.Property(p => p.KeyFeatures)
                    .HasConversion(StringListConverter, StringListComparer)
                    .HasColumnType("nvarchar(max)");
                entity.Property(p => p.PackageContents)
                    .HasConversion(StringListConverter, StringListComparer)
                    .HasColumnType("nvarchar(max)");
                entity.Property(p => p.Specifications)
                    .HasConversion(SpecListConverter, SpecListComparer)
                    .HasColumnType("nvarchar(max)");
                entity.Property(p => p.RowVersion).IsRowVersion();
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

                entity.HasOne(p => p.WarehouseZone)
                    .WithMany(zone => zone.Products)
                    .HasForeignKey(p => p.WarehouseZoneId)
                    .OnDelete(DeleteBehavior.SetNull);
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
                entity.Property(o => o.ServicesTotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Discount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.CreatedAt).HasColumnType("datetime2");
                entity.Property(o => o.UpdatedAt).HasColumnType("datetime2");
                entity.Property(o => o.PaidAt).HasColumnType("datetime2");

                entity.HasOne(o => o.User)
                    .WithMany(user => user.Orders)
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
                    .WithMany(product => product.OrderItems)
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

            modelBuilder.Entity<PasswordResetTokenDomain>(entity =>
            {
                entity.Property(t => t.TokenHash).HasMaxLength(128).IsRequired();
                entity.Property(t => t.CreatedAt).HasColumnType("datetime2");
                entity.Property(t => t.ExpiresAt).HasColumnType("datetime2");
                entity.Property(t => t.ConsumedAt).HasColumnType("datetime2");
                entity.Ignore(t => t.IsUsable);

                entity.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.TokenHash).IsUnique();
                entity.HasIndex(t => t.UserId);
            });

            modelBuilder.Entity<ContactMessageDomain>(entity =>
            {
                entity.Property(m => m.Name).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Email).HasMaxLength(200).IsRequired();
                entity.Property(m => m.Subject).HasMaxLength(200).IsRequired();
                entity.Property(m => m.Message).HasMaxLength(4000).IsRequired();
                entity.Property(m => m.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(m => m.CreatedAt).HasColumnType("datetime2");

                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(m => m.CreatedAt);
                entity.HasIndex(m => m.IsRead);
            });

            modelBuilder.Entity<ReturnDomain>(entity =>
            {
                entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");
                entity.Property(r => r.CreatedAt).HasColumnType("datetime2");
                entity.Property(r => r.UpdatedAt).HasColumnType("datetime2");
                entity.Property(r => r.ResolvedAt).HasColumnType("datetime2");

                entity.HasOne(r => r.Order)
                    .WithMany()
                    .HasForeignKey(r => r.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(r => r.OrderId);
                entity.HasIndex(r => r.Status);
                entity.HasIndex(r => r.CreatedAt);
            });

            modelBuilder.Entity<CouponDomain>(entity =>
            {
                entity.Property(c => c.Code).HasMaxLength(40).IsRequired();
                entity.Property(c => c.DiscountType).HasConversion<string>().HasMaxLength(50);
                entity.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
                entity.Property(c => c.CreatedAt).HasColumnType("datetime2");
                entity.Property(c => c.UpdatedAt).HasColumnType("datetime2");
                entity.Property(c => c.ExpiresAt).HasColumnType("datetime2");

                entity.HasIndex(c => c.Code).IsUnique();
            });

            modelBuilder.Entity<SupportTicketDomain>(entity =>
            {
                entity.Property(ticket => ticket.Subject).HasMaxLength(200).IsRequired();
                entity.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(ticket => ticket.Priority).HasConversion<string>().HasMaxLength(50);
                entity.Property(ticket => ticket.SatisfactionComment).HasMaxLength(1000);
                entity.Property(ticket => ticket.CreatedAt).HasColumnType("datetime2");
                entity.Property(ticket => ticket.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(ticket => ticket.Customer)
                    .WithMany()
                    .HasForeignKey(ticket => ticket.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ticket => ticket.AssignedAgent)
                    .WithMany()
                    .HasForeignKey(ticket => ticket.AssignedAgentId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(ticket => ticket.Status);
                entity.HasIndex(ticket => ticket.CreatedAt);
            });

            modelBuilder.Entity<SupportMessageDomain>(entity =>
            {
                entity.Property(message => message.Text).HasMaxLength(4000).IsRequired();
                entity.Property(message => message.CreatedAt).HasColumnType("datetime2");
                entity.Property(message => message.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(message => message.Ticket)
                    .WithMany(ticket => ticket.Messages)
                    .HasForeignKey(message => message.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(message => message.AuthorUser)
                    .WithMany()
                    .HasForeignKey(message => message.AuthorUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(message => message.TicketId);
            });

            modelBuilder.Entity<SupportAttachmentDomain>(entity =>
            {
                entity.Property(attachment => attachment.FileName).HasMaxLength(260).IsRequired();
                entity.Property(attachment => attachment.FilePath).HasMaxLength(500).IsRequired();
                entity.Property(attachment => attachment.CreatedAt).HasColumnType("datetime2");
                entity.Property(attachment => attachment.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(attachment => attachment.Ticket)
                    .WithMany(ticket => ticket.Attachments)
                    .HasForeignKey(attachment => attachment.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(attachment => attachment.UploadedByUser)
                    .WithMany()
                    .HasForeignKey(attachment => attachment.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ServiceTariffDomain>(entity =>
            {
                entity.Property(tariff => tariff.Name).HasMaxLength(200).IsRequired();
                entity.Property(tariff => tariff.Description).HasMaxLength(1000);
                entity.Property(tariff => tariff.Price).HasColumnType("decimal(18,2)");
                entity.Property(tariff => tariff.CreatedAt).HasColumnType("datetime2");
                entity.Property(tariff => tariff.UpdatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<ServiceRequestDomain>(entity =>
            {
                entity.Property(request => request.RequestNumber).HasMaxLength(32);
                entity.Property(request => request.ServiceTitle).HasMaxLength(200);
                entity.Property(request => request.Description).HasMaxLength(1000);
                entity.Property(request => request.Address).HasMaxLength(400);
                entity.Property(request => request.ContactPhone).HasMaxLength(50);
                entity.Property(request => request.CompletionReport).HasMaxLength(2000);
                entity.Property(request => request.RatingComment).HasMaxLength(1000);
                entity.Property(request => request.Price).HasColumnType("decimal(18,2)");
                entity.Property(request => request.Status).HasConversion<string>();
                entity.Property(request => request.CreatedAt).HasColumnType("datetime2");
                entity.Property(request => request.UpdatedAt).HasColumnType("datetime2");
                entity.Property(request => request.PaidAt).HasColumnType("datetime2");
                entity.HasIndex(request => request.RequestNumber).IsUnique();

                entity.HasOne(request => request.ServiceTariff)
                    .WithMany()
                    .HasForeignKey(request => request.ServiceTariffId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(request => request.Customer)
                    .WithMany(user => user.ServiceRequests)
                    .HasForeignKey(request => request.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(request => request.Installer)
                    .WithMany(user => user.AssignedServiceRequests)
                    .HasForeignKey(request => request.InstallerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(request => request.Manager)
                    .WithMany(user => user.ManagedServiceRequests)
                    .HasForeignKey(request => request.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ServiceRequestCommentDomain>(entity =>
            {
                entity.Property(comment => comment.Message).HasMaxLength(1000);
                entity.Property(comment => comment.CreatedAt).HasColumnType("datetime2");
                entity.Property(comment => comment.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(comment => comment.ServiceRequest)
                    .WithMany(request => request.Comments)
                    .HasForeignKey(comment => comment.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(comment => comment.AuthorUser)
                    .WithMany(user => user.ServiceRequestComments)
                    .HasForeignKey(comment => comment.AuthorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WorkPhotoDomain>(entity =>
            {
                entity.Property(photo => photo.FileName).HasMaxLength(260);
                entity.Property(photo => photo.FilePath).HasMaxLength(500);
                entity.Property(photo => photo.CreatedAt).HasColumnType("datetime2");
                entity.Property(photo => photo.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(photo => photo.ServiceRequest)
                    .WithMany(request => request.WorkPhotos)
                    .HasForeignKey(photo => photo.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationDomain>(entity =>
            {
                entity.Property(notification => notification.Title).HasMaxLength(200);
                entity.Property(notification => notification.Message).HasMaxLength(1000);
                entity.Property(notification => notification.RelatedEntityType).HasMaxLength(100);
                entity.Property(notification => notification.CreatedAt).HasColumnType("datetime2");
                entity.Property(notification => notification.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(notification => notification.User)
                    .WithMany(user => user.Notifications)
                    .HasForeignKey(notification => notification.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ActionLogDomain>(entity =>
            {
                entity.Property(log => log.ActorRole).HasMaxLength(50);
                entity.Property(log => log.EntityType).HasMaxLength(100);
                entity.Property(log => log.Action).HasMaxLength(100);
                entity.Property(log => log.Details).HasMaxLength(2000);
                entity.Property(log => log.CreatedAt).HasColumnType("datetime2");
                entity.Property(log => log.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(log => log.ActorUser)
                    .WithMany(user => user.ActionLogs)
                    .HasForeignKey(log => log.ActorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentTransactionDomain>(entity =>
            {
                entity.Property(payment => payment.Type).HasConversion<string>();
                entity.Property(payment => payment.Status).HasConversion<string>();
                entity.Property(payment => payment.Method).HasConversion<string>();
                entity.Property(payment => payment.Amount).HasColumnType("decimal(18,2)");
                entity.Property(payment => payment.Currency).HasMaxLength(3);
                entity.Property(payment => payment.ExternalReference).HasMaxLength(100);
                entity.Property(payment => payment.Description).HasMaxLength(500);
                entity.Property(payment => payment.CreatedAt).HasColumnType("datetime2");
                entity.Property(payment => payment.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(payment => payment.User)
                    .WithMany(user => user.PaymentTransactions)
                    .HasForeignKey(payment => payment.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(payment => payment.Order)
                    .WithMany(order => order.PaymentTransactions)
                    .HasForeignKey(payment => payment.OrderId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(payment => payment.UserId);
                entity.HasIndex(payment => payment.OrderId);
                entity.HasIndex(payment => payment.CreatedAt);
            });

            modelBuilder.Entity<IdempotencyRecordDomain>(entity =>
            {
                entity.Property(r => r.Key).HasMaxLength(100).IsRequired();
                entity.Property(r => r.Method).HasMaxLength(10).IsRequired();
                entity.Property(r => r.Path).HasMaxLength(200).IsRequired();
                entity.Property(r => r.ContentType).HasMaxLength(100).IsRequired();
                entity.Property(r => r.ResponseBody).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(r => r.CreatedAt).HasColumnType("datetime2");
                entity.Property(r => r.ExpiresAt).HasColumnType("datetime2");

                entity.HasIndex(r => new { r.UserId, r.Key }).IsUnique();
                entity.HasIndex(r => r.ExpiresAt);
            });

            modelBuilder.Entity<ProductReviewDomain>(entity =>
            {
                entity.Property(review => review.Comment).HasMaxLength(2000).IsRequired();
                entity.Property(review => review.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(review => review.ModeratorReply).HasMaxLength(2000);
                entity.Property(review => review.CreatedAt).HasColumnType("datetime2");
                entity.Property(review => review.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(review => review.Product)
                    .WithMany(product => product.Reviews)
                    .HasForeignKey(review => review.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(review => review.User)
                    .WithMany(user => user.ProductReviews)
                    .HasForeignKey(review => review.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(review => review.ModeratorUser)
                    .WithMany()
                    .HasForeignKey(review => review.ModeratorUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(review => review.ProductId);
                entity.HasIndex(review => new { review.UserId, review.ProductId }).IsUnique();
            });

            modelBuilder.Entity<ProductReviewPhotoDomain>(entity =>
            {
                entity.Property(photo => photo.FileName).HasMaxLength(260).IsRequired();
                entity.Property(photo => photo.FilePath).HasMaxLength(500).IsRequired();
                entity.Property(photo => photo.CreatedAt).HasColumnType("datetime2");
                entity.Property(photo => photo.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(photo => photo.Review)
                    .WithMany(review => review.Photos)
                    .HasForeignKey(photo => photo.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductReviewReportDomain>(entity =>
            {
                entity.Property(report => report.Reason).HasMaxLength(1000).IsRequired();
                entity.Property(report => report.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(report => report.CreatedAt).HasColumnType("datetime2");
                entity.Property(report => report.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(report => report.Review)
                    .WithMany(review => review.Reports)
                    .HasForeignKey(report => report.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(report => report.ReporterUser)
                    .WithMany()
                    .HasForeignKey(report => report.ReporterUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReviewResponseTemplateDomain>(entity =>
            {
                entity.Property(template => template.Name).HasMaxLength(120).IsRequired();
                entity.Property(template => template.Body).HasMaxLength(2000).IsRequired();
                entity.Property(template => template.CreatedAt).HasColumnType("datetime2");
                entity.Property(template => template.UpdatedAt).HasColumnType("datetime2");
            });

            ConfigureWarehouse(modelBuilder);
            ConfigureSupportWorkflow(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureWarehouse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WarehouseDomain>(entity =>
            {
                entity.Property(warehouse => warehouse.Name).HasMaxLength(120).IsRequired();
                entity.Property(warehouse => warehouse.Code).HasMaxLength(40).IsRequired();
                entity.Property(warehouse => warehouse.Address).HasMaxLength(300);
                entity.Property(warehouse => warehouse.CreatedAt).HasColumnType("datetime2");
                entity.Property(warehouse => warehouse.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(warehouse => warehouse.Code).IsUnique();
            });

            modelBuilder.Entity<WarehouseZoneDomain>(entity =>
            {
                entity.Property(zone => zone.Name).HasMaxLength(120).IsRequired();
                entity.Property(zone => zone.Code).HasMaxLength(40).IsRequired();
                entity.Property(zone => zone.Description).HasMaxLength(300);
                entity.Property(zone => zone.CreatedAt).HasColumnType("datetime2");
                entity.Property(zone => zone.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(zone => new { zone.WarehouseId, zone.Code }).IsUnique();
            });

            modelBuilder.Entity<ProductStockReservationDomain>(entity =>
            {
                entity.Property(reservation => reservation.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(reservation => reservation.CreatedAt).HasColumnType("datetime2");
                entity.Property(reservation => reservation.UpdatedAt).HasColumnType("datetime2");

                entity.HasOne(reservation => reservation.Product)
                    .WithMany(product => product.StockReservations)
                    .HasForeignKey(reservation => reservation.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(reservation => reservation.Order)
                    .WithMany(order => order.StockReservations)
                    .HasForeignKey(reservation => reservation.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StockMovementDomain>(entity =>
            {
                entity.Property(movement => movement.Type).HasConversion<string>().HasMaxLength(50);
                entity.Property(movement => movement.ReferenceType).HasMaxLength(100);
                entity.Property(movement => movement.Note).HasMaxLength(500);
                entity.Property(movement => movement.CreatedAt).HasColumnType("datetime2");
                entity.Property(movement => movement.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(movement => movement.ProductId);
                entity.HasIndex(movement => movement.CreatedAt);
            });

            modelBuilder.Entity<InventoryCountDomain>(entity =>
            {
                entity.Property(count => count.CountNumber).HasMaxLength(40).IsRequired();
                entity.Property(count => count.Note).HasMaxLength(500);
                entity.Property(count => count.CreatedAt).HasColumnType("datetime2");
                entity.Property(count => count.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(count => count.CountNumber).IsUnique();
            });

            modelBuilder.Entity<InventoryCountItemDomain>(entity =>
            {
                entity.HasOne(item => item.InventoryCount)
                    .WithMany(count => count.Items)
                    .HasForeignKey(item => item.InventoryCountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PurchaseOrderDomain>(entity =>
            {
                entity.Property(order => order.OrderNumber).HasMaxLength(40).IsRequired();
                entity.Property(order => order.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(order => order.Notes).HasMaxLength(1000);
                entity.Property(order => order.CreatedAt).HasColumnType("datetime2");
                entity.Property(order => order.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(order => order.OrderNumber).IsUnique();
            });

            modelBuilder.Entity<PurchaseOrderItemDomain>(entity =>
            {
                entity.Property(item => item.UnitCost).HasColumnType("decimal(18,2)");
                entity.HasOne(item => item.PurchaseOrder)
                    .WithMany(order => order.Items)
                    .HasForeignKey(item => item.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GoodsReceiptDomain>(entity =>
            {
                entity.Property(receipt => receipt.ReceiptNumber).HasMaxLength(40).IsRequired();
                entity.Property(receipt => receipt.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(receipt => receipt.Notes).HasMaxLength(1000);
                entity.Property(receipt => receipt.CreatedAt).HasColumnType("datetime2");
                entity.Property(receipt => receipt.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(receipt => receipt.ReceiptNumber).IsUnique();
            });

            modelBuilder.Entity<GoodsReceiptItemDomain>(entity =>
            {
                entity.Property(item => item.Note).HasMaxLength(500);
                entity.HasOne(item => item.GoodsReceipt)
                    .WithMany(receipt => receipt.Items)
                    .HasForeignKey(item => item.GoodsReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SupplierReturnDomain>(entity =>
            {
                entity.Property(supplierReturn => supplierReturn.ReturnNumber).HasMaxLength(40).IsRequired();
                entity.Property(supplierReturn => supplierReturn.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(supplierReturn => supplierReturn.Reason).HasMaxLength(1000).IsRequired();
                entity.Property(supplierReturn => supplierReturn.CreatedAt).HasColumnType("datetime2");
                entity.Property(supplierReturn => supplierReturn.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(supplierReturn => supplierReturn.ReturnNumber).IsUnique();
            });

            modelBuilder.Entity<SupplierReturnItemDomain>(entity =>
            {
                entity.Property(item => item.Reason).HasMaxLength(500);
                entity.HasOne(item => item.SupplierReturn)
                    .WithMany(supplierReturn => supplierReturn.Items)
                    .HasForeignKey(item => item.SupplierReturnId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WarehouseTransferDomain>(entity =>
            {
                entity.Property(transfer => transfer.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(transfer => transfer.Note).HasMaxLength(500);
                entity.Property(transfer => transfer.CreatedAt).HasColumnType("datetime2");
                entity.Property(transfer => transfer.UpdatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<WarehouseDocumentDomain>(entity =>
            {
                entity.Property(document => document.Type).HasConversion<string>().HasMaxLength(50);
                entity.Property(document => document.Number).HasMaxLength(40).IsRequired();
                entity.Property(document => document.RelatedEntityType).HasMaxLength(120);
                entity.Property(document => document.Title).HasMaxLength(200).IsRequired();
                entity.Property(document => document.Content).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(document => document.CreatedAt).HasColumnType("datetime2");
                entity.Property(document => document.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(document => document.Number).IsUnique();
            });
        }

        private static void ConfigureSupportWorkflow(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeBaseArticleDomain>(entity =>
            {
                entity.Property(article => article.Title).HasMaxLength(200).IsRequired();
                entity.Property(article => article.Category).HasMaxLength(120).IsRequired();
                entity.Property(article => article.Content).HasMaxLength(4000).IsRequired();
                entity.Property(article => article.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(article => article.CreatedAt).HasColumnType("datetime2");
                entity.Property(article => article.UpdatedAt).HasColumnType("datetime2");
                entity.HasIndex(article => article.Status);
            });

            modelBuilder.Entity<WarrantyClaimDomain>(entity =>
            {
                entity.Property(claim => claim.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(claim => claim.IssueDescription).HasMaxLength(1000).IsRequired();
                entity.Property(claim => claim.Resolution).HasMaxLength(1000);
                entity.Property(claim => claim.CreatedAt).HasColumnType("datetime2");
                entity.Property(claim => claim.UpdatedAt).HasColumnType("datetime2");
            });

            modelBuilder.Entity<RemoteDiagnosticSessionDomain>(entity =>
            {
                entity.Property(session => session.Status).HasConversion<string>().HasMaxLength(50);
                entity.Property(session => session.Notes).HasMaxLength(2000);
                entity.Property(session => session.Result).HasMaxLength(2000);
                entity.Property(session => session.CreatedAt).HasColumnType("datetime2");
                entity.Property(session => session.UpdatedAt).HasColumnType("datetime2");
            });
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
