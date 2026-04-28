using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.Tests.TestInfrastructure
{
    internal sealed class FakeProductRepository : IProductRepository
    {
        public List<ProductsDomain> Products { get; } = new();

        public Task<List<ProductsDomain>> GetAllAsync() => Task.FromResult(Products);

        public Task<ProductsDomain?> GetByIdAsync(int id) =>
            Task.FromResult(Products.FirstOrDefault(product => product.Id == id));

        public Task<ProductsDomain> CreateAsync(ProductsDomain product)
        {
            product.Id = Products.Count + 1;
            Products.Add(product);
            return Task.FromResult(product);
        }

        public Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product)
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

        public Task<ProductsDomain?> DeleteAsync(int id)
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

        public Task<List<CategoryDomain>> GetAllAsync() => Task.FromResult(Categories);

        public Task<CategoryDomain?> GetByIdAsync(int id) =>
            Task.FromResult(Categories.FirstOrDefault(category => category.Id == id));

        public Task<bool> ExistsAsync(int id) =>
            Task.FromResult(Categories.Any(category => category.Id == id));

        public Task<bool> HasProductsAsync(int id) =>
            Task.FromResult(HasProductsValue);

        public Task<CategoryDomain> CreateAsync(CategoryDomain category)
        {
            category.Id = Categories.Count + 1;
            Categories.Add(category);
            return Task.FromResult(category);
        }

        public Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category)
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

        public Task<CategoryDomain?> DeleteAsync(int id)
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

        public Task<List<SupplierDomain>> GetAllAsync() => Task.FromResult(Suppliers);

        public Task<SupplierDomain?> GetByIdAsync(int id) =>
            Task.FromResult(Suppliers.FirstOrDefault(supplier => supplier.Id == id));

        public Task<bool> ExistsAsync(int id) =>
            Task.FromResult(Suppliers.Any(supplier => supplier.Id == id));

        public Task<bool> HasProductsAsync(int id) =>
            Task.FromResult(HasProductsValue);

        public Task<SupplierDomain> CreateAsync(SupplierDomain supplier)
        {
            supplier.Id = Suppliers.Count + 1;
            Suppliers.Add(supplier);
            return Task.FromResult(supplier);
        }

        public Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier)
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

        public Task<SupplierDomain?> DeleteAsync(int id)
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

        public Task<List<UserDomain>> GetAllAsync() =>
            Task.FromResult(Users.OrderByDescending(user => user.CreatedAt).ToList());

        public Task<UserDomain?> GetByIdAsync(int id) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Id == id));

        public Task<UserDomain> CreateAsync(UserDomain user)
        {
            user.Id = Users.Count + 1;
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword)
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

        public Task<UserDomain?> DeleteAsync(int id)
        {
            var existing = Users.FirstOrDefault(user => user.Id == id);
            if (existing is not null)
            {
                Users.Remove(existing);
            }

            return Task.FromResult(existing);
        }

        public Task<UserDomain?> GetByUsernameAsync(string username) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Username == username));

        public Task<bool> ExistsByEmailAsync(string email, int? excludedUserId = null) =>
            Task.FromResult(Users.Any(user =>
                user.Email == email &&
                (!excludedUserId.HasValue || user.Id != excludedUserId.Value)));

        public Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId = null) =>
            Task.FromResult(Users.Any(user =>
                user.Username == username &&
                (!excludedUserId.HasValue || user.Id != excludedUserId.Value)));
    }
}
