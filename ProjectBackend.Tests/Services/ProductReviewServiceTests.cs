using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.BusinessLogic.Services;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class ProductReviewServiceTests
    {
        private static ProductReviewService CreateService(
            FakeProductReviewRepository reviewRepo,
            FakeProductRepository productRepo,
            FakeCurrentUserContext currentUser)
        {
            return new ProductReviewService(
                reviewRepo,
                productRepo,
                currentUser,
                new FakeAttachmentStorageService(),
                TestMapperFactory.Create());
        }

        private static FakeProductRepository ProductRepoWith(int productId)
        {
            var repo = new FakeProductRepository();
            repo.Products.Add(new ProductsDomain { Id = productId, Name = "Test", Price = 10 });
            return repo;
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowUnauthorized_WhenUserIsAnonymous()
        {
            var service = CreateService(
                new FakeProductReviewRepository(),
                ProductRepoWith(1),
                new FakeCurrentUserContext());

            var dto = new CreateProductReviewDto { Rating = 5, Comment = "Great" };

            await Assert.ThrowsAsync<UnauthorizedAppException>(
                () => service.CreateAsync(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowNotFound_WhenProductMissing()
        {
            var service = CreateService(
                new FakeProductReviewRepository(),
                new FakeProductRepository(),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            var dto = new CreateProductReviewDto { Rating = 4, Comment = "Nice" };

            await Assert.ThrowsAsync<NotFoundException>(
                () => service.CreateAsync(99, dto, CancellationToken.None));
        }

        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)6)]
        public async Task CreateAsync_ShouldRejectOutOfRangeRating(byte rating)
        {
            var service = CreateService(
                new FakeProductReviewRepository(),
                ProductRepoWith(1),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            var dto = new CreateProductReviewDto { Rating = rating, Comment = "Some text" };

            await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidation_WhenCommentIsBlank()
        {
            var service = CreateService(
                new FakeProductReviewRepository(),
                ProductRepoWith(1),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            var dto = new CreateProductReviewDto { Rating = 5, Comment = "   " };

            await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowConflict_WhenUserAlreadyReviewedProduct()
        {
            var reviewRepo = new FakeProductReviewRepository();
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 1,
                ProductId = 1,
                UserId = 7,
                Rating = 5,
                Comment = "First",
            });

            var service = CreateService(
                reviewRepo,
                ProductRepoWith(1),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            var dto = new CreateProductReviewDto { Rating = 4, Comment = "Second attempt" };

            await Assert.ThrowsAsync<ConflictException>(
                () => service.CreateAsync(1, dto, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAsync_ShouldPersistReview_ForValidPayload()
        {
            var reviewRepo = new FakeProductReviewRepository();
            var service = CreateService(
                reviewRepo,
                ProductRepoWith(1),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            var dto = new CreateProductReviewDto { Rating = 5, Comment = "  Highly recommended  " };

            var result = await service.CreateAsync(1, dto, CancellationToken.None);

            Assert.Single(reviewRepo.Reviews);
            Assert.Equal(7, reviewRepo.Reviews[0].UserId);
            Assert.Equal(1, reviewRepo.Reviews[0].ProductId);
            Assert.Equal("Highly recommended", reviewRepo.Reviews[0].Comment);
            Assert.Equal((byte)5, result.Rating);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowUnauthorized_WhenAnonymous()
        {
            var service = CreateService(
                new FakeProductReviewRepository(),
                new FakeProductRepository(),
                new FakeCurrentUserContext());

            await Assert.ThrowsAsync<UnauthorizedAppException>(
                () => service.DeleteAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowUnauthorized_WhenNotOwnerAndNotAdmin()
        {
            var reviewRepo = new FakeProductReviewRepository();
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 1,
                ProductId = 1,
                UserId = 7,
                Rating = 5,
                Comment = "Mine",
            });

            var service = CreateService(
                reviewRepo,
                new FakeProductRepository(),
                new FakeCurrentUserContext { UserId = 8, Role = UserRole.Customer });

            await Assert.ThrowsAsync<UnauthorizedAppException>(
                () => service.DeleteAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemove_WhenOwner()
        {
            var reviewRepo = new FakeProductReviewRepository();
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 1,
                ProductId = 1,
                UserId = 7,
                Rating = 5,
                Comment = "Mine",
            });

            var service = CreateService(
                reviewRepo,
                new FakeProductRepository(),
                new FakeCurrentUserContext { UserId = 7, Role = UserRole.Customer });

            await service.DeleteAsync(1, CancellationToken.None);

            Assert.Empty(reviewRepo.Reviews);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemove_WhenAdminButNotOwner()
        {
            var reviewRepo = new FakeProductReviewRepository();
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 1,
                ProductId = 1,
                UserId = 7,
                Rating = 5,
                Comment = "Not mine",
            });

            var service = CreateService(
                reviewRepo,
                new FakeProductRepository(),
                new FakeCurrentUserContext { UserId = 99, Role = UserRole.Admin });

            await service.DeleteAsync(1, CancellationToken.None);

            Assert.Empty(reviewRepo.Reviews);
        }

        [Fact]
        public async Task ListByProductAsync_ShouldReturnReviewsOrderedByNewest()
        {
            var reviewRepo = new FakeProductReviewRepository();
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 1,
                ProductId = 1,
                UserId = 7,
                Rating = 4,
                Comment = "Older",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });
            reviewRepo.Reviews.Add(new ProductReviewDomain
            {
                Id = 2,
                ProductId = 1,
                UserId = 8,
                Rating = 5,
                Comment = "Newer",
                CreatedAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            });

            var service = CreateService(
                reviewRepo,
                new FakeProductRepository(),
                new FakeCurrentUserContext());

            var result = await service.ListByProductAsync(1, CancellationToken.None);

            Assert.Collection(result,
                first => Assert.Equal("Newer", first.Comment),
                second => Assert.Equal("Older", second.Comment));
        }
    }
}
