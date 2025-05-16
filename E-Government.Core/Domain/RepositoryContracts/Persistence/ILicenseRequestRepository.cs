using E_Government.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Persistence
{
    public interface ILicenseRequestRepository
    {
        Task<IEnumerable<ILicenseRequest>> GetAllAsync();
    }
}
