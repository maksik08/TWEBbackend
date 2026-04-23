using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ProjectDbContext _dbContext;

        public SupplierRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SupplierDomain>> GetAllAsync()
        {
            return await _dbContext.Suppliers.ToListAsync();
        }

        public async Task<SupplierDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SupplierDomain> CreateAsync(SupplierDomain supplier)
        {
            await _dbContext.Suppliers.AddAsync(supplier);
            await _dbContext.SaveChangesAsync();
            return supplier;
        }

        public async Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier)
        {
            var existing = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (existing is null) return null;

            existing.Name = supplier.Name;
            existing.ContactEmail = supplier.ContactEmail;
            existing.Phone = supplier.Phone;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<SupplierDomain?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
            if (existing is null) return null;

            _dbContext.Suppliers.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
