using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Query;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.BusinessLogic.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProjectBackend.Tests.TestInfrastructure
{
    internal sealed class FakeProductRepository : IProductRepository
    {
        public List<ProductsDomain> Products { get; } = new();

        public Task<PagedResult<ProductsDomain>> GetAllAsync(ProductQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var items = queryOptions.IncludeHidden
                ? Products.ToList()
                : Products.Where(product => product.IsVisible).ToList();

            return Task.FromResult(new PagedResult<ProductsDomain>
            {
                Items = items,
                TotalCount = items.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });
        }

        public Task<IReadOnlyCollection<ProductsDomain>> GetByIdsAsync(
            IReadOnlyCollection<int> ids,
            bool includeHidden,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ProductsDomain> items = Products
                .Where(product => ids.Contains(product.Id) && (includeHidden || product.IsVisible))
                .ToList();

            return Task.FromResult(items);
        }

        public Task<IReadOnlyCollection<ProductsDomain>> GetTrackedByIdsAsync(
            IReadOnlyCollection<int> ids,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ProductsDomain> items = Products
                .Where(product => ids.Contains(product.Id))
                .ToList();

            return Task.FromResult(items);
        }

        public Task<IReadOnlyCollection<ProductsDomain>> GetAllForStockReportAsync(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ProductsDomain> items = Products.ToList();
            return Task.FromResult(items);
        }

        public Task<ProductsDomain?> SetVisibilityAsync(int id, bool isVisible, CancellationToken cancellationToken)
        {
            var existing = Products.FirstOrDefault(product => product.Id == id);
            if (existing is not null)
            {
                existing.IsVisible = isVisible;
            }

            return Task.FromResult(existing);
        }

        public Task<ProductsDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Products.FirstOrDefault(product => product.Id == id));

        public Task<ProductsDomain> CreateAsync(ProductsDomain product, CancellationToken cancellationToken)
        {
            product.Id = Products.Count + 1;
            Products.Add(product);
            return Task.FromResult(product);
        }

        public Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product, CancellationToken cancellationToken)
        {
            var existing = Products.FirstOrDefault(item => item.Id == id);
            if (existing is null)
            {
                return Task.FromResult<ProductsDomain?>(null);
            }

            existing.Name = product.Name;
            existing.Title = product.Title;
            existing.Image = product.Image;
            existing.Brand = product.Brand;
            existing.Sku = product.Sku;
            existing.ShortDescription = product.ShortDescription;
            existing.Description = product.Description;
            existing.Warranty = product.Warranty;
            existing.Availability = product.Availability;
            existing.Technology = product.Technology;
            existing.KeyFeatures = product.KeyFeatures;
            existing.PackageContents = product.PackageContents;
            existing.Specifications = product.Specifications;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;
            existing.IsPreorder = product.IsPreorder;
            existing.IsVisible = product.IsVisible;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;
            return Task.FromResult<ProductsDomain?>(existing);
        }

        public Task<ProductsDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = Products.FirstOrDefault(item => item.Id == id);
            if (existing is not null)
            {
                Products.Remove(existing);
            }

            return Task.FromResult(existing);
        }
    }

    internal sealed class FakeProductReviewRepository : IProductReviewRepository
    {
        public List<ProductReviewDomain> Reviews { get; } = new();

        public Task<IReadOnlyCollection<ProductReviewDomain>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ProductReviewDomain> items = Reviews
                .Where(review => review.ProductId == productId)
                .OrderByDescending(review => review.CreatedAt)
                .ToList();
            return Task.FromResult(items);
        }

        public Task<ProductReviewDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Reviews.FirstOrDefault(review => review.Id == id));

        public Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken cancellationToken) =>
            Task.FromResult(Reviews.Any(review => review.UserId == userId && review.ProductId == productId));

        public Task<ProductReviewDomain> CreateAsync(ProductReviewDomain review, CancellationToken cancellationToken)
        {
            review.Id = Reviews.Count + 1;
            Reviews.Add(review);
            return Task.FromResult(review);
        }

        public Task DeleteAsync(ProductReviewDomain review, CancellationToken cancellationToken)
        {
            Reviews.Remove(review);
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeCategoryRepository : ICategoryRepository
    {
        public List<CategoryDomain> Categories { get; } = new();
        public bool HasProductsValue { get; set; }

        public Task<PagedResult<CategoryDomain>> GetAllAsync(CategoryQueryOptions queryOptions, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<CategoryDomain>
            {
                Items = Categories,
                TotalCount = Categories.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });

        public Task<CategoryDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.FirstOrDefault(category => category.Id == id));

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.Any(category => category.Id == id));

        public Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.Any(category =>
                category.Name == name &&
                (!excludedId.HasValue || category.Id != excludedId.Value)));

        public Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(HasProductsValue);

        public Task<CategoryDomain> CreateAsync(CategoryDomain category, CancellationToken cancellationToken)
        {
            category.Id = Categories.Count + 1;
            Categories.Add(category);
            return Task.FromResult(category);
        }

        public Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category, CancellationToken cancellationToken)
        {
            var existing = Categories.FirstOrDefault(item => item.Id == id);
            if (existing is null)
            {
                return Task.FromResult<CategoryDomain?>(null);
            }

            existing.Name = category.Name;
            existing.Description = category.Description;
            return Task.FromResult<CategoryDomain?>(existing);
        }

        public Task<CategoryDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = Categories.FirstOrDefault(item => item.Id == id);
            if (existing is not null)
            {
                Categories.Remove(existing);
            }

            return Task.FromResult(existing);
        }
    }

    internal sealed class FakeSupplierRepository : ISupplierRepository
    {
        public List<SupplierDomain> Suppliers { get; } = new();
        public bool HasProductsValue { get; set; }

        public Task<PagedResult<SupplierDomain>> GetAllAsync(SupplierQueryOptions queryOptions, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<SupplierDomain>
            {
                Items = Suppliers,
                TotalCount = Suppliers.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });

        public Task<SupplierDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Suppliers.FirstOrDefault(supplier => supplier.Id == id));

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Suppliers.Any(supplier => supplier.Id == id));

        public Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken) =>
            Task.FromResult(Suppliers.Any(supplier =>
                supplier.Name == name &&
                (!excludedId.HasValue || supplier.Id != excludedId.Value)));

        public Task<bool> ExistsByContactEmailAsync(string email, int? excludedId, CancellationToken cancellationToken) =>
            Task.FromResult(Suppliers.Any(supplier =>
                supplier.ContactEmail == email &&
                (!excludedId.HasValue || supplier.Id != excludedId.Value)));

        public Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(HasProductsValue);

        public Task<SupplierDomain> CreateAsync(SupplierDomain supplier, CancellationToken cancellationToken)
        {
            supplier.Id = Suppliers.Count + 1;
            Suppliers.Add(supplier);
            return Task.FromResult(supplier);
        }

        public Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier, CancellationToken cancellationToken)
        {
            var existing = Suppliers.FirstOrDefault(item => item.Id == id);
            if (existing is null)
            {
                return Task.FromResult<SupplierDomain?>(null);
            }

            existing.Name = supplier.Name;
            existing.ContactEmail = supplier.ContactEmail;
            existing.Phone = supplier.Phone;
            return Task.FromResult<SupplierDomain?>(existing);
        }

        public Task<SupplierDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = Suppliers.FirstOrDefault(item => item.Id == id);
            if (existing is not null)
            {
                Suppliers.Remove(existing);
            }

            return Task.FromResult(existing);
        }
    }

    internal sealed class FakeUserRepository : IUserRepository
    {
        public List<UserDomain> Users { get; } = new();

        public Task<PagedResult<UserDomain>> GetAllAsync(UserQueryOptions queryOptions, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<UserDomain>
            {
                Items = Users.OrderByDescending(user => user.CreatedAt).ToList(),
                TotalCount = Users.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });

        public Task<UserDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Id == id));

        public Task<UserDomain> CreateAsync(UserDomain user, CancellationToken cancellationToken)
        {
            user.Id = Users.Count + 1;
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(item => item.Id == id);
            if (existing is null)
            {
                return Task.FromResult<UserDomain?>(null);
            }

            existing.Email = user.Email;
            existing.Username = user.Username;
            existing.Role = user.Role;

            if (updatePassword)
            {
                existing.Password = user.Password;
            }

            return Task.FromResult<UserDomain?>(existing);
        }

        public Task<UserDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(user => user.Id == id);
            if (existing is not null)
            {
                Users.Remove(existing);
            }

            return Task.FromResult(existing);
        }

        public Task<UserDomain?> SetBlockedAsync(int id, bool isBlocked, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(user => user.Id == id);
            if (existing is not null)
            {
                existing.IsBlocked = isBlocked;
            }

            return Task.FromResult(existing);
        }

        public Task<UserDomain?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Username == username));

        public Task<UserDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Email == email));

        public Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(user => user.Id == userId);
            if (existing is not null)
            {
                existing.Password = passwordHash;
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<UserDomain>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<UserDomain> users = Users
                .Where(user => user.Role == role)
                .OrderBy(user => user.Username)
                .ToList();

            return Task.FromResult(users);
        }

        public Task<int> CountByRoleAsync(UserRole role, CancellationToken cancellationToken) =>
            Task.FromResult(Users.Count(user => user.Role == role));

        public Task<bool> ExistsByEmailAsync(string email, int? excludedUserId, CancellationToken cancellationToken) =>
            Task.FromResult(Users.Any(user =>
                user.Email == email &&
                (!excludedUserId.HasValue || user.Id != excludedUserId.Value)));

        public Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId, CancellationToken cancellationToken) =>
            Task.FromResult(Users.Any(user =>
                user.Username == username &&
                (!excludedUserId.HasValue || user.Id != excludedUserId.Value)));

        public Task<UserDomain?> UpdateProfileAsync(int id, string? firstName, string? lastName, string? phone, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(user => user.Id == id);
            if (existing is null)
            {
                return Task.FromResult<UserDomain?>(null);
            }

            existing.FirstName = firstName;
            existing.LastName = lastName;
            existing.Phone = phone;
            return Task.FromResult<UserDomain?>(existing);
        }

        public Task<UserDomain?> AdjustBalanceAsync(int id, decimal delta, CancellationToken cancellationToken)
        {
            var existing = Users.FirstOrDefault(user => user.Id == id);
            if (existing is null)
            {
                return Task.FromResult<UserDomain?>(null);
            }

            existing.Balance += delta;
            return Task.FromResult<UserDomain?>(existing);
        }
    }

    internal sealed class FakeOrderRepository : IOrderRepository
    {
        public List<OrderDomain> Orders { get; } = new();

        public Task<PagedResult<OrderDomain>> GetAllAsync(OrderQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            IEnumerable<OrderDomain> items = Orders;

            if (queryOptions.UserId.HasValue)
            {
                items = items.Where(order => order.UserId == queryOptions.UserId.Value);
            }

            if (queryOptions.Status.HasValue)
            {
                items = items.Where(order => order.Status == queryOptions.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                items = items.Where(order =>
                    (order.User?.Username?.Contains(queryOptions.Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (order.RecipientName?.Contains(queryOptions.Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (order.Phone?.Contains(queryOptions.Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (order.ShippingAddress?.Contains(queryOptions.Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (order.City?.Contains(queryOptions.Search, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            var resultItems = items.ToList();
            return Task.FromResult(new PagedResult<OrderDomain>
            {
                Items = resultItems,
                TotalCount = resultItems.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });
        }

        public Task<OrderDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Orders.FirstOrDefault(order => order.Id == id));

        public Task<OrderDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Orders.FirstOrDefault(order => order.Id == id));

        public Task<OrderDomain> CreateAsync(OrderDomain order, CancellationToken cancellationToken)
        {
            order.Id = Orders.Count + 1;
            foreach (var item in order.Items)
            {
                item.Id = item.Id == 0 ? order.Items.ToList().IndexOf(item) + 1 : item.Id;
                item.OrderId = order.Id;
            }

            Orders.Add(order);
            return Task.FromResult(order);
        }

        public Task<OrderDomain> UpdateAsync(OrderDomain order, CancellationToken cancellationToken) =>
            Task.FromResult(order);

        public Task<IReadOnlyCollection<OrderDomain>> GetAllForReportAsync(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<OrderDomain> items = Orders.ToList();
            return Task.FromResult(items);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IDbContextTransaction>(new FakeDbContextTransaction());
    }

    internal sealed class FakeServiceRequestRepository : IServiceRequestRepository
    {
        public List<ServiceRequestDomain> Requests { get; } = new();

        public Task<PagedResult<ServiceRequestDomain>> GetAllAsync(ServiceRequestQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            IEnumerable<ServiceRequestDomain> items = Requests;

            if (queryOptions.CustomerId.HasValue)
            {
                items = items.Where(request => request.CustomerId == queryOptions.CustomerId.Value);
            }

            if (queryOptions.InstallerId.HasValue)
            {
                items = items.Where(request => request.InstallerId == queryOptions.InstallerId.Value);
            }

            if (queryOptions.Status.HasValue)
            {
                items = items.Where(request => request.Status == queryOptions.Status.Value);
            }

            if (queryOptions.ScheduledFrom.HasValue)
            {
                items = items.Where(request => request.ScheduledVisitAt >= queryOptions.ScheduledFrom.Value);
            }

            if (queryOptions.ScheduledTo.HasValue)
            {
                items = items.Where(request => request.ScheduledVisitAt <= queryOptions.ScheduledTo.Value);
            }

            var resultItems = items.ToList();
            return Task.FromResult(new PagedResult<ServiceRequestDomain>
            {
                Items = resultItems,
                TotalCount = resultItems.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });
        }

        public Task<ServiceRequestDomain?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Requests.FirstOrDefault(request => request.Id == id));

        public Task<ServiceRequestDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(Requests.FirstOrDefault(request => request.Id == id));

        public Task<ServiceRequestDomain> CreateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken)
        {
            serviceRequest.Id = Requests.Count + 1;
            Requests.Add(serviceRequest);
            return Task.FromResult(serviceRequest);
        }

        public Task<ServiceRequestDomain> UpdateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken) =>
            Task.FromResult(serviceRequest);

        public Task<ServiceRequestDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = Requests.FirstOrDefault(request => request.Id == id);
            if (existing is not null)
            {
                Requests.Remove(existing);
            }

            return Task.FromResult(existing);
        }

        public Task<IReadOnlyCollection<ServiceRequestDomain>> GetAllForReportAsync(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<ServiceRequestDomain> items = Requests.ToList();
            return Task.FromResult(items);
        }
    }

    internal sealed class FakeActionLogService : IActionLogService
    {
        public List<(string Action, string EntityType, int? EntityId, string? Details)> Entries { get; } = new();

        public Task RecordAsync(string action, string entityType, int? entityId, string? details, CancellationToken cancellationToken)
        {
            Entries.Add((action, entityType, entityId, details));
            return Task.CompletedTask;
        }

        public Task<PagedResult<ActionLogDto>> GetAllAsync(ActionLogListRequestDto request, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<ActionLogDto>
            {
                Items = Entries
                    .Select((entry, index) => new ActionLogDto
                    {
                        Id = index + 1,
                        Action = entry.Action,
                        EntityType = entry.EntityType,
                        EntityId = entry.EntityId,
                        Details = entry.Details
                    })
                    .ToList(),
                TotalCount = Entries.Count,
                Page = request.Page,
                PageSize = request.PageSize
            });
    }

    internal sealed class FakeNotificationService : INotificationService
    {
        public List<(int UserId, string Title, string Message, string? EntityType, int? EntityId)> Notifications { get; } = new();

        public Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(NotificationListRequestDto request, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<NotificationDto>
            {
                Items = Array.Empty<NotificationDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize
            });

        public Task<NotificationDto> MarkAsReadAsync(int id, CancellationToken cancellationToken) =>
            Task.FromResult(new NotificationDto { Id = id, IsRead = true });

        public Task NotifyAsync(int userId, string title, string message, string? relatedEntityType, int? relatedEntityId, CancellationToken cancellationToken)
        {
            Notifications.Add((userId, title, message, relatedEntityType, relatedEntityId));
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeWorkPhotoStorageService : IWorkPhotoStorageService
    {
        public List<StoredWorkPhotoResult> StoredPhotos { get; } = new();

        public Task<IReadOnlyCollection<StoredWorkPhotoResult>> SaveAsync(ICollection<Microsoft.AspNetCore.Http.IFormFile> files, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<StoredWorkPhotoResult> results = files
                .Select(file => new StoredWorkPhotoResult(file.FileName, $"/uploads/test/{file.FileName}"))
                .ToList();

            StoredPhotos.AddRange(results);
            return Task.FromResult(results);
        }
    }

    internal sealed class FakePaymentTransactionService : IPaymentTransactionService
    {
        public List<PaymentTransactionDto> Payments { get; } = new();

        public Task<PagedResult<PaymentTransactionDto>> GetMyPaymentsAsync(PaymentTransactionListRequestDto request, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PaymentTransactionDto> items = Payments.ToList();
            return Task.FromResult(new PagedResult<PaymentTransactionDto>
            {
                Items = items,
                TotalCount = items.Count,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }

        public Task<PagedResult<PaymentTransactionDto>> GetAllAsync(PaymentTransactionListRequestDto request, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<PaymentTransactionDto> items = Payments.ToList();
            return Task.FromResult(new PagedResult<PaymentTransactionDto>
            {
                Items = items,
                TotalCount = items.Count,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }

        public Task<PaymentTransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Payments.First(payment => payment.Id == id));
        }

        public Task<PaymentTransactionDto> RecordAsync(
            int userId,
            decimal amount,
            PaymentTransactionType type,
            PaymentMethod method,
            PaymentTransactionStatus status,
            int? orderId,
            string? description,
            string? externalReference,
            CancellationToken cancellationToken)
        {
            var payment = new PaymentTransactionDto
            {
                Id = Payments.Count + 1,
                UserId = userId,
                OrderId = orderId,
                Amount = amount,
                Type = type,
                Method = method,
                Status = status,
                Description = description,
                ExternalReference = externalReference
            };

            Payments.Add(payment);
            return Task.FromResult(payment);
        }
    }

    internal sealed class FakeImageStorageService : IImageStorageService
    {
        public Task<string> SaveProductImageAsync(Microsoft.AspNetCore.Http.IFormFile file, CancellationToken cancellationToken)
        {
            return Task.FromResult($"/uploads/test/{file.FileName}");
        }

        public bool TryDeleteProductImage(string? imagePath)
        {
            return true;
        }
    }

    internal sealed class FakeCurrentUserContext : ICurrentUserContext
    {
        public int? UserId { get; init; }

        public string? Username { get; init; }

        public UserRole? Role { get; init; }
    }

    internal sealed class FakeDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId { get; } = Guid.NewGuid();

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public void Dispose()
        {
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
