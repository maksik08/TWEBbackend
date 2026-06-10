using AutoMapper;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ProductReviewService : ApplicationServiceBase, IProductReviewService
    {
        private readonly IProductReviewRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IAttachmentStorageService _attachmentStorageService;
        private readonly IMapper _mapper;

        public ProductReviewService(
            IProductReviewRepository repository,
            IProductRepository productRepository,
            ICurrentUserContext currentUser,
            IAttachmentStorageService attachmentStorageService,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _currentUser = currentUser;
            _attachmentStorageService = attachmentStorageService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<ProductReviewDto>> ListByProductAsync(int productId, CancellationToken cancellationToken)
        {
            var reviews = await _repository.GetByProductIdAsync(productId, cancellationToken);
            return _mapper.Map<IReadOnlyCollection<ProductReviewDto>>(reviews);
        }

        public async Task<PagedResult<ProductReviewDto>> ListByProductAsync(
            int productId,
            ProductReviewListRequestDto request,
            CancellationToken cancellationToken)
        {
            var reviews = await _repository.GetAllAsync(BuildQuery(productId, request, includeHidden: IsModerator()), cancellationToken);
            return MapPagedReviews(reviews);
        }

        public async Task<PagedResult<ProductReviewDto>> GetModerationQueueAsync(
            ProductReviewListRequestDto request,
            CancellationToken cancellationToken)
        {
            EnsureModerator();
            var reviews = await _repository.GetAllAsync(BuildQuery(null, request, includeHidden: true), cancellationToken);
            return MapPagedReviews(reviews);
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
                Status = ProductReviewStatus.Published
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<ProductReviewDto>(created);
        }

        public async Task<ProductReviewPhotoDto> AddPhotoAsync(int reviewId, UploadProductReviewPhotoDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var review = EnsureFound(await _repository.GetByIdAsync(reviewId, cancellationToken), "Review", reviewId);
            if (review.UserId != userId && !IsModerator())
            {
                throw new UnauthorizedAppException("You can attach photos only to your own review.");
            }

            var storedFile = await _attachmentStorageService.SaveAsync(dto.File, "review-photos", cancellationToken);
            var photo = await _repository.AddPhotoAsync(new ProductReviewPhotoDomain
            {
                ReviewId = reviewId,
                FileName = storedFile.FileName,
                FilePath = storedFile.RelativePath
            }, cancellationToken);

            return _mapper.Map<ProductReviewPhotoDto>(photo);
        }

        public async Task<ProductReviewReportDto> ReportAsync(int reviewId, ReportProductReviewDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var review = EnsureFound(await _repository.GetByIdAsync(reviewId, cancellationToken), "Review", reviewId);
            if (review.UserId == userId)
            {
                throw new ValidationException("You cannot report your own review.");
            }

            var report = await _repository.AddReportAsync(new ProductReviewReportDomain
            {
                ReviewId = reviewId,
                ReporterUserId = userId,
                Reason = NormalizeRequiredText(dto.Reason, "Report reason"),
                Status = ProductReviewReportStatus.Open
            }, cancellationToken);

            return _mapper.Map<ProductReviewReportDto>(report);
        }

        public async Task<PagedResult<ProductReviewReportDto>> GetReportsAsync(ProductReviewReportStatus? status, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var reports = await _repository.GetReportsAsync(new ProductReviewReportQueryOptions
            {
                Page = 1,
                PageSize = 100,
                SortBy = "createdat",
                SortDescending = true,
                Status = status
            }, cancellationToken);

            return new PagedResult<ProductReviewReportDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ProductReviewReportDto>>(reports.Items),
                TotalCount = reports.TotalCount,
                Page = reports.Page,
                PageSize = reports.PageSize
            };
        }

        public async Task<ProductReviewDto> UpdateStatusAsync(int reviewId, UpdateProductReviewStatusDto dto, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var review = EnsureFound(await _repository.GetTrackedByIdAsync(reviewId, cancellationToken), "Review", reviewId);
            review.Status = dto.Status;
            await _repository.CreateTemplateAsync(new ReviewResponseTemplateDomain
            {
                Name = $"Audit review {review.Id} {DateTime.UtcNow:yyyyMMddHHmmss}",
                Body = $"Review status changed to {dto.Status}.",
                IsActive = false
            }, cancellationToken);
            return _mapper.Map<ProductReviewDto>(review);
        }

        public async Task<ProductReviewDto> ReplyAsync(int reviewId, ReplyToProductReviewDto dto, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var moderatorId = RequireUserId();
            var review = EnsureFound(await _repository.GetTrackedByIdAsync(reviewId, cancellationToken), "Review", reviewId);
            review.ModeratorReply = NormalizeRequiredText(dto.Reply, "Reply");
            review.ModeratorUserId = moderatorId;
            review.RepliedAt = DateTime.UtcNow;
            await _repository.CreateTemplateAsync(new ReviewResponseTemplateDomain
            {
                Name = $"Audit reply {review.Id} {DateTime.UtcNow:yyyyMMddHHmmss}",
                Body = "Moderator replied to review.",
                IsActive = false
            }, cancellationToken);
            return _mapper.Map<ProductReviewDto>(review);
        }

        public async Task<PagedResult<ReviewResponseTemplateDto>> GetTemplatesAsync(ListQueryRequestDto request, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var templates = await _repository.GetTemplatesAsync(new PagedQueryOptionsImpl
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = "name",
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection)
            }, cancellationToken);

            return new PagedResult<ReviewResponseTemplateDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ReviewResponseTemplateDto>>(templates.Items),
                TotalCount = templates.TotalCount,
                Page = templates.Page,
                PageSize = templates.PageSize
            };
        }

        public async Task<ReviewResponseTemplateDto> CreateTemplateAsync(CreateReviewResponseTemplateDto dto, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var template = await _repository.CreateTemplateAsync(new ReviewResponseTemplateDomain
            {
                Name = NormalizeRequiredText(dto.Name, "Template name"),
                Body = NormalizeRequiredText(dto.Body, "Template body"),
                IsActive = dto.IsActive
            }, cancellationToken);

            return _mapper.Map<ReviewResponseTemplateDto>(template);
        }

        public async Task<ReviewResponseTemplateDto> UpdateTemplateAsync(int id, UpdateReviewResponseTemplateDto dto, CancellationToken cancellationToken)
        {
            EnsureModerator();
            var template = EnsureFound(await _repository.GetTrackedTemplateAsync(id, cancellationToken), "Review response template", id);
            template.Name = NormalizeRequiredText(dto.Name, "Template name");
            template.Body = NormalizeRequiredText(dto.Body, "Template body");
            template.IsActive = dto.IsActive;
            template = await _repository.UpdateTemplateAsync(template, cancellationToken);
            return _mapper.Map<ReviewResponseTemplateDto>(template);
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

        private int RequireUserId()
        {
            return _currentUser.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");
        }

        private bool IsModerator()
        {
            return _currentUser.Role is UserRole.Admin or UserRole.Moderator;
        }

        private void EnsureModerator()
        {
            if (!IsModerator())
            {
                throw new UnauthorizedAppException("Moderator access is required.");
            }
        }

        private static ProductReviewQueryOptions BuildQuery(int? productId, ProductReviewListRequestDto request, bool includeHidden)
        {
            return new ProductReviewQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat", "rating"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                ProductId = productId,
                Rating = request.Rating,
                Status = request.Status,
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                IncludeHidden = includeHidden
            };
        }

        private PagedResult<ProductReviewDto> MapPagedReviews(PagedResult<ProductReviewDomain> reviews)
        {
            return new PagedResult<ProductReviewDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ProductReviewDto>>(reviews.Items),
                TotalCount = reviews.TotalCount,
                Page = reviews.Page,
                PageSize = reviews.PageSize
            };
        }

        private sealed class PagedQueryOptionsImpl : PagedQueryOptions
        {
        }
    }
}
