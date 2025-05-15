using E_Government.APIs.Controllers.Base;
using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.License
{
    public class LicenseController : ApiControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost("create-license")]
        public async Task<ActionResult<DrivingLicense>> CreateLicense([FromForm] DrivingLicenseDTO licenseDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _licenseService.CreateLicenseAsync(licenseDTO);
            return Ok(result);
        }

        [HttpPost("renew-license")]
        public async Task<ActionResult<DrivingLicenseRenewal>> update(RenewDrivingDTO renewDrivingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _licenseService.RenewLicenseAsync(renewDrivingDTO);
            return Ok(result);
        }

        [HttpPost("vehicle-renew")]
        public async Task<ActionResult<VehicleLicenseRenewal>> VehicleLicenseCreate(VehicleRenwal dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var result = await _licenseService.CreateVehicleLicenseRenewalAsync(dto);
            return Ok(result);
        }

        [HttpPost("traffic-violation")]
        public async Task<ActionResult<TrafficViolationPayment>> CreateViolationAsync(TrafficViolationDTO dto)
        {
            var result = await _licenseService.CreateTrafficViolationAsync(dto);
            return Ok(result);
        }

        [HttpPost("vehicle-license")]
        public async Task<ActionResult<VehicleOwner>> CreateOwnerAsync(VehicleOwnerDTO dto)
        {
            var result = await _licenseService.CreateVehicleOwnerAsync(dto);
            return Ok(result);
        }

        [HttpPost("replacement-license")]
        public async Task<ActionResult<LicenseReplacementRequest>> CreateReplacementAsync(LicenseReplacementDto dto)
        {
            var result = await _licenseService.CreateLicenseReplacementAsync(dto);
            return Ok(result);
        }
    }
}