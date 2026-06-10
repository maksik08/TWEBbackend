using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class SupportTicketService : ApplicationServiceBase, ISupportTicketService
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IActionLogService _actionLogService;
        private readonly IAttachmentStorageService _attachmentStorageService;
        private readonly IMapper _mapper;

        public SupportTicketService(
            ISupportTicketRepository repository,
            IUserRepository userRepository,
            ICurrentUserContext currentUserContext,
            IActionLogService actionLogService,
            IAttachmentStorageService attachmentStorageService,
            IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _currentUserContext = currentUserContext;
            _actionLogService = actionLogService;
            _attachmentStorageService = attachmentStorageService;
            _mapper = mapper;
        }

        public async Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();

            var ticket = new SupportTicketDomain
            {
                Subject = NormalizeRequiredText(dto.Subject, "Subject"),
                Status = SupportTicketStatus.Open,
                CustomerId = userId,
                Messages = new List<SupportMessageDomain>
                {
                    new()
                    {
                        AuthorUserId = userId,
                        Text = NormalizeRequiredText(dto.Message, "Message")
                    }
                }
            };

            var created = await _repository.CreateAsync(ticket, cancellationToken);
            await _actionLogService.RecordAsync(
                "Create",
                nameof(SupportTicketDomain),
                created.Id,
                $"Opened support ticket #{created.Id}: {created.Subject}.",
                cancellationToken);

            return await LoadDtoAsync(created.Id, cancellationToken);
        }

        public Task<PagedResult<SupportTicketDto>> GetMineAsync(SupportTicketListRequestDto request, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            return ListAsync(request, customerId: userId, cancellationToken);
        }

        public Task<PagedResult<SupportTicketDto>> GetAllAsync(SupportTicketListRequestDto request, CancellationToken cancellationToken)
        {
            EnsureStaff();
            return ListAsync(request, customerId: null, cancellationToken);
        }

        public async Task<SupportTicketDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var ticket = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Support ticket", id);
            EnsureCanAccess(ticket);
            return _mapper.Map<SupportTicketDto>(ticket);
        }

        public async Task<SupportTicketDto> PostMessageAsync(int id, PostSupportMessageDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var ticket = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Support ticket", id);

            var isStaff = IsStaff();
            if (ticket.CustomerId != userId && !isStaff)
            {
                throw new NotFoundException($"Support ticket with id {id} was not found.");
            }

            if (ticket.Status == SupportTicketStatus.Closed)
            {
                throw new ValidationException("This ticket is closed. Open a new ticket instead.");
            }

            await _repository.AddMessageAsync(new SupportMessageDomain
            {
                TicketId = ticket.Id,
                AuthorUserId = userId,
                Text = NormalizeRequiredText(dto.Text, "Message")
            }, cancellationToken);

            // Move the ticket along its lifecycle based on who replied.
            if (isStaff && ticket.Status == SupportTicketStatus.Open)
            {
                ticket.Status = SupportTicketStatus.InProgress;
                await _repository.UpdateAsync(ticket, cancellationToken);
            }
            else if (!isStaff && ticket.Status == SupportTicketStatus.Resolved)
            {
                ticket.Status = SupportTicketStatus.InProgress;
                await _repository.UpdateAsync(ticket, cancellationToken);
            }

            return await LoadDtoAsync(ticket.Id, cancellationToken);
        }

        public async Task<SupportTicketDto> UpdateStatusAsync(int id, UpdateSupportTicketStatusDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var ticket = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Support ticket", id);

            ticket.Status = dto.Status;
            await _repository.UpdateAsync(ticket, cancellationToken);

            await _actionLogService.RecordAsync(
                "UpdateStatus",
                nameof(SupportTicketDomain),
                ticket.Id,
                $"Support ticket #{ticket.Id} status set to {dto.Status}.",
                cancellationToken);

            return await LoadDtoAsync(ticket.Id, cancellationToken);
        }

        public async Task<SupportTicketDto> AssignAsync(int id, AssignSupportAgentDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var ticket = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Support ticket", id);

            var agent = EnsureFound(await _userRepository.GetByIdAsync(dto.AgentId, cancellationToken), "User", dto.AgentId);
            if (agent.Role != UserRole.Support && agent.Role != UserRole.Admin)
            {
                throw new ValidationException("Only support agents or administrators can be assigned to a ticket.");
            }

            ticket.AssignedAgentId = agent.Id;
            if (ticket.Status == SupportTicketStatus.Open)
            {
                ticket.Status = SupportTicketStatus.InProgress;
            }

            await _repository.UpdateAsync(ticket, cancellationToken);

            await _actionLogService.RecordAsync(
                "Assign",
                nameof(SupportTicketDomain),
                ticket.Id,
                $"Support ticket #{ticket.Id} assigned to {agent.Username}.",
                cancellationToken);

            return await LoadDtoAsync(ticket.Id, cancellationToken);
        }

        public async Task<SupportTicketDto> EscalateAsync(int id, EscalateSupportTicketDto dto, CancellationToken cancellationToken)
        {
            EnsureStaff();
            var ticket = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Support ticket", id);

            ticket.Priority = dto.Priority;
            ticket.EscalatedAt = DateTime.UtcNow;
            if (ticket.Status == SupportTicketStatus.Open)
            {
                ticket.Status = SupportTicketStatus.InProgress;
            }

            await _repository.UpdateAsync(ticket, cancellationToken);

            await _actionLogService.RecordAsync(
                "Escalate",
                nameof(SupportTicketDomain),
                ticket.Id,
                $"Support ticket #{ticket.Id} escalated to {ticket.Priority}. {NormalizeOptionalText(dto.Reason)}",
                cancellationToken);

            return await LoadDtoAsync(ticket.Id, cancellationToken);
        }

        public async Task<SupportTicketDto> RateAsync(int id, RateSupportTicketDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var ticket = EnsureFound(await _repository.GetTrackedByIdAsync(id, cancellationToken), "Support ticket", id);
            if (ticket.CustomerId != userId)
            {
                throw new NotFoundException($"Support ticket with id {id} was not found.");
            }

            if (ticket.Status is not SupportTicketStatus.Resolved and not SupportTicketStatus.Closed)
            {
                throw new ValidationException("Only resolved or closed tickets can be rated.");
            }

            ticket.SatisfactionRating = dto.Rating;
            ticket.SatisfactionComment = NormalizeOptionalText(dto.Comment);
            await _repository.UpdateAsync(ticket, cancellationToken);
            return await LoadDtoAsync(ticket.Id, cancellationToken);
        }

        public async Task<SupportAttachmentDto> AddAttachmentAsync(int id, UploadSupportAttachmentDto dto, CancellationToken cancellationToken)
        {
            var userId = RequireUserId();
            var ticket = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Support ticket", id);
            EnsureCanAccess(ticket);

            if (ticket.Status == SupportTicketStatus.Closed)
            {
                throw new ValidationException("Cannot attach files to a closed ticket.");
            }

            var storedFile = await _attachmentStorageService.SaveAsync(dto.File, "support-attachments", cancellationToken);
            var attachment = await _repository.AddAttachmentAsync(new SupportAttachmentDomain
            {
                TicketId = id,
                UploadedByUserId = userId,
                FileName = storedFile.FileName,
                FilePath = storedFile.RelativePath
            }, cancellationToken);

            return _mapper.Map<SupportAttachmentDto>(attachment);
        }

        private async Task<PagedResult<SupportTicketDto>> ListAsync(
            SupportTicketListRequestDto request,
            int? customerId,
            CancellationToken cancellationToken)
        {
            var queryOptions = new SupportTicketQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "updatedat", "updatedat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status,
                Priority = request.Priority,
                CustomerId = customerId
            };

            var tickets = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<SupportTicketDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<SupportTicketDto>>(tickets.Items),
                TotalCount = tickets.TotalCount,
                Page = tickets.Page,
                PageSize = tickets.PageSize
            };
        }

        private async Task<SupportTicketDto> LoadDtoAsync(int id, CancellationToken cancellationToken)
        {
            var ticket = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Support ticket", id);
            return _mapper.Map<SupportTicketDto>(ticket);
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

        private void EnsureCanAccess(SupportTicketDomain ticket)
        {
            var userId = RequireUserId();
            if (ticket.CustomerId == userId || IsStaff())
            {
                return;
            }

            throw new NotFoundException($"Support ticket with id {ticket.Id} was not found.");
        }
    }
}
