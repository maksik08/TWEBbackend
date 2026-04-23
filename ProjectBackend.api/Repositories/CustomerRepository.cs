using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ProjectDbContext _dbContext;

        public CustomerRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CustomerDomain>> GetAllAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<CustomerDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CustomerDomain> CreateAsync(CustomerDomain customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<CustomerDomain?> UpdateAsync(int id, CustomerDomain customer)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (existing is null) return null;

            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<CustomerDomain?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (existing is null) return null;

            _dbContext.Customers.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
