using AutoMapper;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ProductReviewService : ApplicationServiceBase, IProductReviewService
    {
        private readonly IProductReviewRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IMapper _mapper;

        public ProductReviewService(
            IProductReviewRepository repository,
            IProductRepository productRepository,
            ICurrentUserContext currentUser,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<ProductReviewDto>> ListByProductAsync(int productId, CancellationToken cancellationToken)
        {
            var reviews = await _repository.GetByProductIdAsync(productId, cancellationToken);
            return _mapper.Map<IReadOnlyCollection<ProductReviewDto>>(reviews);
        }

        public async Task<ProductReviewDto> CreateAsync(int productId, CreateProductReviewDto dto, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAppException("Authentication is required to post a review.");

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Product with id {productId} was not found.");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new ValidationException("Rating must be between 1 and 5.");
            }

            var comment = NormalizeRequiredText(dto.Comment, "Comment");

            if (await _repository.ExistsByUserAndProductAsync(userId, productId, cancellationToken))
            {
                throw new ConflictException("You have already reviewed this product. Delete the existing review before posting a new one.");
            }

            var entity = new ProductReviewDomain
            {
                ProductId = productId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = comment,
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<ProductReviewDto>(created);
        }

        public async Task DeleteAsync(int reviewId, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAppException("Authentication is required to delete a review.");

            var review = EnsureFound(await _repository.GetByIdAsync(reviewId, cancellationToken), "Review", reviewId);

            var isOwner = review.UserId == userId;
            var isAdmin = _currentUser.Role == UserRole.Admin;
            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAppException("You can only delete your own review.");
            }

            await _repository.DeleteAsync(review, cancellationToken);
        }
    }
}
