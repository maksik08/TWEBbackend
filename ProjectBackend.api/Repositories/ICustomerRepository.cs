using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<CustomerDomain>> GetAllAsync();
        Task<CustomerDomain?> GetByIdAsync(int id);
        Task<CustomerDomain> CreateAsync(CustomerDomain customer);
        Task<CustomerDomain?> UpdateAsync(int id, CustomerDomain customer);
        Task<CustomerDomain?> DeleteAsync(int id);
    }
}
