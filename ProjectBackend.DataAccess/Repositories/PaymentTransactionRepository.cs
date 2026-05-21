using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly ProjectDbContext _dbContext;

        public PaymentTransactionRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<PaymentTransactionDomain>> GetForUserAsync(
            int userId,
            PaymentTransactionQueryOptions queryOptions,
            CancellationToken cancellationToken)
        {
            var query = BuildQuery().Where(payment => payment.UserId == userId);
            query = ApplyFilters(query, queryOptions);
            query = ApplySort(query, queryOptions);
            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<PagedResult<PaymentTransactionDomain>> GetAllAsync(
            PaymentTransactionQueryOptions queryOptions,
            CancellationToken cancellationToken)
        {
            var query = BuildQuery();
            query = ApplyFilters(query, queryOptions);
            query = ApplySort(query, queryOptions);
            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<PaymentTransactionDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await BuildQuery()
                .FirstOrDefaultAsync(payment => payment.Id == id, cancellationToken);
        }

        public async Task<PaymentTransactionDomain> CreateAsync(PaymentTransactionDomain paymentTransaction, CancellationToken cancellationToken)
        {
            await _dbContext.PaymentTransactions.AddAsync(paymentTransaction, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return paymentTransaction;
        }

        private IQueryable<PaymentTransactionDomain> BuildQuery()
        {
            return _dbContext.PaymentTransactions
                .AsNoTracking()
                .Include(payment => payment.User)
                .Include(payment => payment.Order);
        }

        private static IQueryable<PaymentTransactionDomain> ApplyFilters(
            IQueryable<PaymentTransactionDomain> query,
            PaymentTransactionQueryOptions queryOptions)
        {
            if (queryOptions.Type.HasValue)
            {
                query = query.Where(payment => payment.Type == queryOptions.Type.Value);
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(payment => payment.Status == queryOptions.Status.Value);
            }

            if (queryOptions.OrderId.HasValue)
            {
                query = query.Where(payment => payment.OrderId == queryOptions.OrderId.Value);
            }

            return query;
        }

        private static IQueryable<PaymentTransactionDomain> ApplySort(
            IQueryable<PaymentTransactionDomain> query,
            PaymentTransactionQueryOptions queryOptions)
        {
            return queryOptions.SortBy switch
            {
                "amount" => queryOptions.SortDescending
                    ? query.OrderByDescending(payment => payment.Amount).ThenByDescending(payment => payment.Id)
                    : query.OrderBy(payment => payment.Amount).ThenBy(payment => payment.Id),
                "type" => queryOptions.SortDescending
                    ? query.OrderByDescending(payment => payment.Type).ThenByDescending(payment => payment.Id)
                    : query.OrderBy(payment => payment.Type).ThenBy(payment => payment.Id),
                "status" => queryOptions.SortDescending
                    ? query.OrderByDescending(payment => payment.Status).ThenByDescending(payment => payment.Id)
                    : query.OrderBy(payment => payment.Status).ThenBy(payment => payment.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(payment => payment.CreatedAt).ThenByDescending(payment => payment.Id)
                    : query.OrderBy(payment => payment.CreatedAt).ThenBy(payment => payment.Id)
            };
        }
    }
}
