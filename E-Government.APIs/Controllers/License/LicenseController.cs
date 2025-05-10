using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.UI.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Admin.DashBoard.Helpers;

namespace E_Government.UI.Controllers.License
{
 
    public class LicenseController : ApiControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LicenseController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [HttpPost("create-license")]
        public async Task<ActionResult<DrivingLicense>> CreateLicense([FromForm]DrivingLicenseDTO licenseDTO)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var photo = ImageSettings.Upload(licenseDTO.photo, "License");
            var model = new DrivingLicense
            {
                  Name = licenseDTO.Name,
                  NID = licenseDTO.NID,
                  DateOfBirth = licenseDTO.DateOfBirth,
                  Address = licenseDTO.Address,
                  LicenseType = licenseDTO.LicenseType,
                  MedicalTest=licenseDTO.MedicalTest,
                  TheoryTest=licenseDTO.TheoryTest,
                  PracticalTest=licenseDTO.TheoryTest,
                  ExpiryDate=licenseDTO.ExpiryDate,
                  IssueDate=licenseDTO.IssueDate,
                  photo=photo,
            };

        await _unitOfWork.GetRepository<DrivingLicense, int>().AddAsync(model);
        await    _unitOfWork.CompleteAsync();
            return Ok();
        }


        [HttpPost("renew-license")]
        public async Task<ActionResult<DrivingLicenseRenewal>> update(RenewDrivingDTO renewDrivingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var photo = ImageSettings.Upload(renewDrivingDTO.NewPhoto, "DrivingLicensePhoto");
            var model = new DrivingLicenseRenewal
            {
                    NID = renewDrivingDTO.NID,
                    PaymentMethod = renewDrivingDTO.PaymentMethod,
                    CurrentExpiryDate = renewDrivingDTO.CurrentExpiryDate,
                    CurrentLicenseNumber = renewDrivingDTO.CurrentLicenseNumber,
                     MedicalCheckRequired = renewDrivingDTO.MedicalCheckRequired,
                     NewExpirayDate = renewDrivingDTO.NewExpirayDate,
                     NewPhoto = photo,
                     RenewalDate = renewDrivingDTO.RenewalDate
            };

           await _unitOfWork.GetRepository<DrivingLicenseRenewal, int>().AddAsync(model);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
        [HttpPost("vehicle-renew")]
        public async Task<ActionResult<VehicleLicenseRenewal> >VehicleLicenseCreate(VehicleRenwal dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var vehicleInfo = new VehicleLicenseRenewal
            {
                PlateNumber = dto.PlateNumber,
                VehicleRegistrationNumber = dto.VehicleRegistrationNumber,
                TechnicalInspectionReport = dto.TechnicalInspectionReport,
                InsuranceDocument = dto.InsuranceDocument,
                PendingFines = dto.PendingFines,
                PaymentMethod = dto.PaymentMethod,
                RenewalDate = dto.RenewalDate
            };

            
          await  _unitOfWork.GetRepository<VehicleLicenseRenewal, int>().AddAsync(vehicleInfo);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
        [HttpPost("traffic-violation")]
        public async Task<ActionResult<TrafficViolationPayment>> CreateViolationAsync(TrafficViolationDTO dto)
        {
           

            var violation = new TrafficViolationPayment
            {
                PlateNumber = dto.PlateNumber,
                ViolationType = dto.ViolationType,
                FineAmount = dto.FineAmount,
                ViolationDate = dto.ViolationDate,
                PaymentStatus = dto.PaymentStatus,
                PaymentMethod = dto.PaymentMethod,
                PaymentReceipt = dto.PaymentReceipt
            };

       await   _unitOfWork.GetRepository<TrafficViolationPayment, int>().AddAsync(violation);
            await _unitOfWork.CompleteAsync();


            return Ok();
        }

        [HttpPost("vehicle-license")]
        public async Task<ActionResult<VehicleOwner>> CreateOwnerAsync(VehicleOwnerDTO dto)
        {
           

            var owner = new VehicleOwner
            {
                NationalId = dto.NationalId,
                OwnerName = dto.OwnerName,
                VehicleType = dto.VehicleType,
                Model = dto.Model,
                ManufactureYear = dto.ManufactureYear,
                Color = dto.Color,
                ChassisNumber = dto.ChassisNumber,
                InspectionReport = dto.InspectionReport,
                InsuranceDocument = dto.InsuranceDocument,
                OwnershipProof = dto.OwnershipProof
            };

          await  _unitOfWork.GetRepository<VehicleOwner, int>().AddAsync(owner);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
        [HttpPost("replacement-license")]
        public async Task<ActionResult<LicenseReplacementRequest>> CreateReplacementAsync(LicenseReplacementDto dto)
        {
         

            var request = new LicenseReplacementRequest
            {
                LicenseType = dto.LicenseType,
                OriginalLicenseNumber = dto.OriginalLicenseNumber,
                Reason = dto.Reason,
                PoliceReport = dto.PoliceReport,
                DamagedLicensePhoto = dto.DamagedLicensePhoto,
                ReplacementFee = dto.ReplacementFee,
                PaymentMethod = dto.PaymentMethod
            };

           await _unitOfWork.GetRepository<LicenseReplacementRequest, int>().AddAsync(request);
            await _unitOfWork.CompleteAsync();

            return Ok();
        }
    }
}
