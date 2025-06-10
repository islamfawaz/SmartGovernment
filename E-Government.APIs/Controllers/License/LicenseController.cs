using E_Government.APIs.Controllers.Base;
using E_Government.Application.DTO.License;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.ServiceContracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.License
{
    public class LicenseController : ApiControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public LicenseController(IServiceManager serviceManager)
        {
           _serviceManager = serviceManager;
        }

        [HttpPost("create-license")]
        public async Task<ActionResult<DrivingLicense>> CreateLicense([FromForm] DrivingLicenseDTO licenseDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceManager.LicenseService.CreateLicenseAsync(licenseDTO);
            return Ok(result);
        }

        [HttpPost("renew-license")]
        public async Task<ActionResult<DrivingLicenseRenewal>> update(RenewDrivingDTO renewDrivingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceManager.LicenseService.RenewLicenseAsync(renewDrivingDTO);
            return Ok(result);
        }

        [HttpPost("vehicle-renew")]
        public async Task<ActionResult<VehicleLicenseRenewal>> VehicleLicenseCreate(VehicleRenwal dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var result = await  _serviceManager.LicenseService.CreateVehicleLicenseRenewalAsync(dto);
            return Ok(result);
        }

        [HttpPost("traffic-violation")]
        public async Task<ActionResult<TrafficViolationPayment>> CreateViolationAsync(TrafficViolationDTO dto)
        {
            var result = await _serviceManager.LicenseService.CreateTrafficViolationAsync(dto);
            return Ok(result);
        }

        [HttpPost("vehicle-license")]
        public async Task<ActionResult<VehicleOwner>> CreateOwnerAsync(VehicleOwnerDTO dto)
        {
            var result = await _serviceManager.LicenseService.CreateVehicleOwnerAsync(dto);
            return Ok(result);
        }

        [HttpPost("replacement-license")]
        public async Task<ActionResult<LicenseReplacementRequest>> CreateReplacementAsync(LicenseReplacementDto dto)
        {
            var result = await _serviceManager.LicenseService.CreateLicenseReplacementAsync(dto);
            return Ok(result);
        }
    }
}