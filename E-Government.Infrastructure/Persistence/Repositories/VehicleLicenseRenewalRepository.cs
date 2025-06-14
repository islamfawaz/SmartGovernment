using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.RepositoryContracts;
using E_Government.Domain.RepositoryContracts.Persistence;

namespace E_Government.Infrastructure.Persistence.Repositories
{
    public class VehicleLicenseRenewalRepository : ILicenseRequestRepository
    {
        private readonly IGenericRepository<VehicleLicenseRenewal, int> _genericRepository;

        public VehicleLicenseRenewalRepository(IGenericRepository<VehicleLicenseRenewal,int> genericRepository)
        {
           _genericRepository = genericRepository;
        }
        public async Task<IEnumerable<ILicenseRequest>> GetAllAsync()
        {
            var data=await _genericRepository.GetAllAsync();
            return data.Cast<ILicenseRequest>();
        }
    }
}
