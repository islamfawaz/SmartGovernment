using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.RepositoryContracts;
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Domain.ServiceContracts;

namespace E_Government.Infrastructure.Generic_Repository
{
  public  class LicenseReplacementRequestRepository :ILicenseRequestRepository
    {
        private readonly IGenericRepository<LicenseReplacementRequest, int> _genericRepository;

        public LicenseReplacementRequestRepository(IGenericRepository<LicenseReplacementRequest, int> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<IEnumerable<ILicenseRequest>> GetAllAsync()
        {
            var data = await _genericRepository.GetAllAsync();
            return data.Cast<ILicenseRequest>();
        }
    }
}
