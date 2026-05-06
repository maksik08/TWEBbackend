using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;
using ProjectBackend.api.Services;

namespace ProjectBackend.Tests.TestInfrastructure
{
    internal sealed class FakeProductRepository : IProductRepository
    {
        public List<ProductsDomain> Products { get; } = new();

        public Task<PagedResult<ProductsDomain>> GetAllAsync(ProductQueryOptions queryOptions, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<ProductsDomain>
            {
                Items = Products,
                TotalCount = Products.Count,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            });

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
            existing.Price = product.Price;
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

        public Task<UserDomain?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Username == username));

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
    }

    internal sealed class FakeCurrentUserContext : ICurrentUserContext
    {
        public int? UserId { get; init; }
    }
}
