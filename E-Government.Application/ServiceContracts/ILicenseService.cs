using E_Government.Application.DTO.License;
using E_Government.Domain.Entities.Liscenses;

namespace E_Government.Application.ServiceContracts
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