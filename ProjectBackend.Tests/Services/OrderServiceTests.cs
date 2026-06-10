using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;
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
            var paymentTransactionService = new FakePaymentTransactionService();
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
                new FakeCouponRepository(),
                paymentTransactionService,
                new FakeWarehouseOperationsService(),
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
            Assert.Empty(paymentTransactionService.Payments);
        }

        [Fact]
        public async Task PayAsync_ShouldRecordCompletedPaymentTransaction()
        {
            var orderRepository = new FakeOrderRepository();
            var userRepository = new FakeUserRepository();
            var productRepository = new FakeProductRepository();
            var paymentTransactionService = new FakePaymentTransactionService();
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
                IsVisible = true
            });

            orderRepository.Orders.Add(new OrderDomain
            {
                Id = 3,
                UserId = 9,
                Status = OrderStatus.Pending,
                Subtotal = 200,
                Items =
                [
                    new OrderItemDomain
                    {
                        Id = 1,
                        OrderId = 3,
                        ProductId = 1,
                        ProductName = "Router",
                        Quantity = 2,
                        UnitPrice = 100
                    }
                ]
            });

            var warehouseOperationsService = new FakeWarehouseOperationsService();
            var service = new OrderService(
                orderRepository,
                userRepository,
                productRepository,
                new FakeCouponRepository(),
                paymentTransactionService,
                warehouseOperationsService,
                new FakeCurrentUserContext { UserId = 9, Role = UserRole.User, Username = "client" },
                actionLogService,
                TestMapperFactory.Create());

            var paid = await service.PayAsync(3, CancellationToken.None);

            Assert.Equal(OrderStatus.Paid, paid.Status);
            Assert.Single(paymentTransactionService.Payments);
            Assert.Equal(PaymentTransactionType.OrderPayment, paymentTransactionService.Payments[0].Type);
            Assert.Equal(200, paymentTransactionService.Payments[0].Amount);
            Assert.Contains(3, warehouseOperationsService.ConsumedOrders);
        }
    }
}
