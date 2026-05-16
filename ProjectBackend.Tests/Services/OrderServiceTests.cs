using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreatePendingOrderWithoutChangingStock()
        {
            var orderRepository = new FakeOrderRepository();
            var userRepository = new FakeUserRepository();
            var productRepository = new FakeProductRepository();
            var actionLogService = new FakeActionLogService();

            userRepository.Users.Add(new UserDomain
            {
                Id = 9,
                Email = "client@test.com",
                Username = "client",
                Password = "hashed",
                Role = UserRole.User,
                Balance = 500
            });

            productRepository.Products.Add(new ProductsDomain
            {
                Id = 1,
                Name = "Router",
                Price = 100,
                StockQuantity = 5,
                IsVisible = true,
                IsPreorder = false
            });

            var service = new OrderService(
                orderRepository,
                userRepository,
                productRepository,
                new FakeCurrentUserContext { UserId = 9, Role = UserRole.User, Username = "client" },
                actionLogService,
                TestMapperFactory.Create());

            var dto = new CreateOrderDto
            {
                RecipientName = "John Client",
                Phone = "123456",
                ShippingAddress = "Main street 1",
                City = "Chisinau",
                Items =
                [
                    new CreateOrderItemDto
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                ]
            };

            var created = await service.CreateAsync(dto, CancellationToken.None);

            Assert.Equal(200, created.Subtotal);
            Assert.Equal(OrderStatus.Pending, created.Status);
            Assert.Single(created.Items);
            Assert.Equal(5, productRepository.Products.Single().StockQuantity);
            Assert.Single(orderRepository.Orders);
            Assert.Single(actionLogService.Entries);
        }
    }
}
