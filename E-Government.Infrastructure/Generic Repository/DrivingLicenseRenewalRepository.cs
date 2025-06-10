using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Domain.ServiceContracts;
using E_Government.Domain.Entities.Liscenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Government.Domain.RepositoryContracts;

namespace E_Government.Infrastructure.Generic_Repository
{
   public class DrivingLicenseRenewalRepository : ILicenseRequestRepository
    {
        private readonly IGenericRepository<DrivingLicense, int> _genericRepository;

        public DrivingLicenseRenewalRepository(IGenericRepository<DrivingLicense,int> genericRepository)
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
