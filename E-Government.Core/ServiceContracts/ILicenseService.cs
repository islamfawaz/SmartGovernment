using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace E_Government.Core.ServiceContracts
{
    public interface ILicenseService
    {
        Task<DrivingLicense> CreateLicenseAsync(DrivingLicenseDTO licenseDTO);
        Task<DrivingLicenseRenewal> RenewLicenseAsync(RenewDrivingDTO renewDrivingDTO);
        Task<VehicleLicenseRenewal> CreateVehicleLicenseRenewalAsync(VehicleRenwal dto);
        Task<TrafficViolationPayment> CreateTrafficViolationAsync(TrafficViolationDTO dto);
        Task<VehicleOwner> CreateVehicleOwnerAsync(VehicleOwnerDTO dto);
        Task<LicenseReplacementRequest> CreateLicenseReplacementAsync(LicenseReplacementDto dto);
    }
}