using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
   public interface IAdminService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
        Task<PagedResult<RequestSummaryDto>> GetAllRequestsAsync(int pageNumber, int pageSize, string? status, string? type, string? searchTerm);
        Task<LicenseRequestDetailsDto> GetLicenseRequestDetailsAsync(Guid id);
        Task<CivilDocumentRequestDetailsDto> GetCivilDocumentRequestDetailsAsync(Guid id);
        Task<bool> ApproveLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input);
        Task<bool> RejectLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input);
        Task<bool> ApproveCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input);
        Task<bool> RejectCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input);


    }
}
