using AutoMapper;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.DataAccess.Repositories;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.BusinessLogic.Services
{
    public class SupportWorkflowService : ApplicationServiceBase, ISupportWorkflowService
    {
        private readonly ISupportWorkflowRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IActionLogService _actionLogService;
        private readonly IMapper _mapper;

        public SupportWorkflowService(
            ISupportWorkflowRepository repository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            ISupportTicketRepository supportTicketRepository,
            ICurrentUserContext currentUserContext,
            IActionLogService actionLogService,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _supportTicketRepository = supportTicketRepository;
            _currentUserContext = currentUserContext;
            _actionLogService = actionLogService;
            _mapper = mapper;
        }

        public async Task<PagedResult<KnowledgeBaseArticleDto>> GetArticlesAsync(KnowledgeBaseArticleListRequestDto request, CancellationToken cancellationToken)
        {
            var includeDrafts = IsStaff();
            var articles = await _repository.GetArticlesAsync(new KnowledgeBaseArticleQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "title", "title", "updatedat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Category = NormalizeOptionalText(request.Category),
                Status = request.Status,
                IncludeDrafts = includeDrafts
            }, cancellationToken);

            return new PagedResult<KnowledgeBaseArticleDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<KnowledgeBaseArticleDto>>(articles.Items),
                TotalCount = articles.TotalCount,
                Page = articles.Page,
                PageSize = articles.PageSize
            };
        }

        public async Task<KnowledgeBaseArticleDto> CreateArticleAsync(CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var article = await _repository.CreateArticleAsync(new KnowledgeBaseArticleDomain
            {
                Title = NormalizeRequiredText(dto.Title, "Title"),
                Category = NormalizeRequiredText(dto.Category, "Category"),
                Content = NormalizeRequiredText(dto.Content, "Content"),
                Status = dto.Status
            }, cancellationToken);

            return _mapper.Map<KnowledgeBaseArticleDto>(article);
        }

        public async Task<KnowledgeBaseArticleDto> UpdateArticleAsync(int id, UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var article = EnsureFound(await _repository.GetArticleAsync(id, cancellationToken), "Knowledge base article", id);
            article.Title = NormalizeRequiredText(dto.Title, "Title");
            article.Category = NormalizeRequiredText(dto.Category, "Category");
            article.Content = NormalizeRequiredText(dto.Content, "Content");
            article.Status = dto.Status;
            article = await _repository.UpdateArticleAsync(article, cancellationToken);
            return _mapper.Map<KnowledgeBaseArticleDto>(article);
        }

        public async Task<WarrantyCheckDto> CheckWarrantyAsync(int productId, int? orderId, CancellationToken cancellationToken)
        {
            var product = EnsureFound(await _productRepository.GetByIdAsync(productId, cancellationToken), "Product", productId);
            var purchasedAt = await GetPurchaseDateAsync(productId, orderId, cancellationToken);
            var months = ParseWarrantyMonths(product.Warranty);
            DateTime? expiresAt = purchasedAt.HasValue && months > 0
                ? purchasedAt.Value.AddMonths(months)
                : null;

            return new WarrantyCheckDto
            {
                ProductId = productId,
                OrderId = orderId,
                PurchasedAt = purchasedAt,
                WarrantyExpiresAt = expiresAt,
                WarrantyValid = expiresAt.HasValue && expiresAt.Value >= DateTime.UtcNow,
                WarrantySource = product.Warranty
            };
        }

        public async Task<WarrantyClaimDto> CreateWarrantyClaimAsync(CreateWarrantyClaimDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var check = await CheckWarrantyAsync(dto.ProductId, dto.OrderId, cancellationToken);

            var claim = await _repository.CreateWarrantyClaimAsync(new WarrantyClaimDomain
            {
                CustomerId = userId,
                ProductId = dto.ProductId,
                OrderId = dto.OrderId,
                SupportTicketId = dto.SupportTicketId,
                WarrantyValid = check.WarrantyValid,
                PurchasedAt = check.PurchasedAt,
                WarrantyExpiresAt = check.WarrantyExpiresAt,
                IssueDescription = NormalizeRequiredText(dto.IssueDescription, "Issue description"),
                Status = WarrantyClaimStatus.Open
            }, cancellationToken);

            await _actionLogService.RecordAsync("Create", nameof(WarrantyClaimDomain), claim.Id, "Warranty claim opened.", cancellationToken);
            return _mapper.Map<WarrantyClaimDto>(claim);
        }

        public async Task<WarrantyClaimDto> UpdateWarrantyClaimAsync(int id, UpdateWarrantyClaimStatusDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var claim = EnsureFound(await _repository.GetWarrantyClaimAsync(id, cancellationToken), "Warranty claim", id);
            claim.Status = dto.Status;
            claim.Resolution = NormalizeOptionalText(dto.Resolution);
            claim = await _repository.UpdateWarrantyClaimAsync(claim, cancellationToken);
            return _mapper.Map<WarrantyClaimDto>(claim);
        }

        public async Task<RemoteDiagnosticSessionDto> StartDiagnosticAsync(int ticketId, StartRemoteDiagnosticDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var agentId = RequireUserId();
            EnsureFound(await _supportTicketRepository.GetByIdAsync(ticketId, cancellationToken), "Support ticket", ticketId);

            var session = await _repository.CreateDiagnosticSessionAsync(new RemoteDiagnosticSessionDomain
            {
                SupportTicketId = ticketId,
                AgentId = agentId,
                ScheduledAt = dto.ScheduledAt,
                Notes = NormalizeOptionalText(dto.Notes),
                Status = dto.ScheduledAt.HasValue ? RemoteDiagnosticStatus.Scheduled : RemoteDiagnosticStatus.InProgress
            }, cancellationToken);

            return _mapper.Map<RemoteDiagnosticSessionDto>(session);
        }

        public async Task<RemoteDiagnosticSessionDto> CompleteDiagnosticAsync(int id, CompleteRemoteDiagnosticDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var session = EnsureFound(await _repository.GetDiagnosticSessionAsync(id, cancellationToken), "Remote diagnostic session", id);
            session.Status = RemoteDiagnosticStatus.Completed;
            session.CompletedAt = DateTime.UtcNow;
            session.Result = NormalizeRequiredText(dto.Result, "Result");
            session = await _repository.UpdateDiagnosticSessionAsync(session, cancellationToken);
            return _mapper.Map<RemoteDiagnosticSessionDto>(session);
        }

        private async Task<DateTime?> GetPurchaseDateAsync(int productId, int? orderId, CancellationToken cancellationToken)
        {
            if (!orderId.HasValue)
            {
                return null;
            }

            var order = EnsureFound(await _orderRepository.GetByIdAsync(orderId.Value, cancellationToken), "Order", orderId.Value);
            if (order.UserId != RequireUserId() && !IsStaff())
            {
                throw new NotFoundException($"Order with id {orderId.Value} was not found.");
            }

            if (!order.Items.Any(item => item.ProductId == productId))
            {
                throw new ValidationException("The selected order does not contain this product.");
            }

            return order.PaidAt ?? order.CreatedAt;
        }

        private int RequireUserId()
        {
            return _currentUserContext.UserId
                ?? throw new UnauthorizedAppException("Authentication is required.");
        }

        private bool IsStaff()
        {
            return _currentUserContext.Role is UserRole.Support or UserRole.Admin;
        }

        private void EnsureStaff()
        {
            if (!IsStaff())
            {
                throw new UnauthorizedAppException("Support staff access is required.");
            }
        }

        private static int ParseWarrantyMonths(string? warranty)
        {
            if (string.IsNullOrWhiteSpace(warranty))
            {
                return 0;
            }

            var digits = new string(warranty.Where(char.IsDigit).ToArray());
            if (!int.TryParse(digits, out var value) || value <= 0)
            {
                return 0;
            }

            return warranty.Contains("year", StringComparison.OrdinalIgnoreCase) ||
                   warranty.Contains("год", StringComparison.OrdinalIgnoreCase)
                ? value * 12
                : value;
        }
    }
}
