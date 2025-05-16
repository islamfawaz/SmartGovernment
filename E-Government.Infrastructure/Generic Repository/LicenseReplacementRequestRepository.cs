using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;

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
