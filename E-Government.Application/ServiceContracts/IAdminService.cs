using E_Government.Application.DTO.AdminDashboard;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.DTO.License;
using E_Government.Domain.Entities;

namespace E_Government.Application.ServiceContracts
{
   public interface IAdminService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
        Task<PagedResult<RequestSummaryDto>> GetAllRequestsAsync(int pageNumber, int pageSize, string? status, string? type, string? searchTerm);
        Task<LicenseRequestDetailsDto> GetLicenseRequestDetailsAsync(Guid id);
        Task<CivilDocumentRequestDetailsDto> GetCivilDocumentRequestDetailsAsync(Guid id);
        Task<bool> ApproveCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input);
        Task<bool> RejectCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input);


    }
}
