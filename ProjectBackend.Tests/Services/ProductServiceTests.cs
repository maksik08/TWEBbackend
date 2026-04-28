using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenCategoryDoesNotExist()
        {
            var productRepository = new FakeProductRepository();
            var categoryRepository = new FakeCategoryRepository();
            var supplierRepository = new FakeSupplierRepository();
            supplierRepository.Suppliers.Add(new SupplierDomain { Id = 1, Name = "Supplier" });

            var service = new ProductService(
                productRepository,
                categoryRepository,
                supplierRepository,
                TestMapperFactory.Create());

            var dto = new CreateProductDto
            {
                Name = "Notebook",
                Price = 10,
                CategoryId = 99,
                SupplierId = 1
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            var service = new ProductService(
                new FakeProductRepository(),
                new FakeCategoryRepository(),
                new FakeSupplierRepository(),
                TestMapperFactory.Create());

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(5));
        }
    }
}
