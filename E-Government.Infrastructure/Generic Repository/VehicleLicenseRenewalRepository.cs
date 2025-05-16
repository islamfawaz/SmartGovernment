using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Generic_Repository
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
