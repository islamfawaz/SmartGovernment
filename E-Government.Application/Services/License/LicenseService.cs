using E_Government.Application.DTO.License;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.RepositoryContracts.Persistence;
using Talabat.Admin.DashBoard.Helpers;

namespace E_Government.Application.Services.License
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LicenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DrivingLicense> CreateLicenseAsync(DrivingLicenseDTO licenseDTO)
        {
            var photo = ImageSettings.Upload(licenseDTO.photo, "License");
            var model = new DrivingLicense
            {
                Name = licenseDTO.Name,
                NID = licenseDTO.NID,
                DateOfBirth = licenseDTO.DateOfBirth,
                Address = licenseDTO.Address,
                LicenseType = licenseDTO.LicenseType,
                MedicalTest = licenseDTO.MedicalTest,
                TheoryTest = licenseDTO.TheoryTest,
                PracticalTest = licenseDTO.TheoryTest,
                ExpiryDate = licenseDTO.ExpiryDate,
                IssueDate = licenseDTO.IssueDate,
                photo = photo,
            };

            await _unitOfWork.GetRepository<DrivingLicense, int>().AddAsync(model);
            await _unitOfWork.CompleteAsync();
            return model;
        }

        public async Task<DrivingLicenseRenewal> RenewLicenseAsync(RenewDrivingDTO renewDrivingDTO)
        {
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
            return model;
        }

        public async Task<VehicleLicenseRenewal> CreateVehicleLicenseRenewalAsync(VehicleRenwal dto)
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

            await _unitOfWork.GetRepository<VehicleLicenseRenewal, int>().AddAsync(vehicleInfo);
            await _unitOfWork.CompleteAsync();
            return vehicleInfo;
        }

        public async Task<TrafficViolationPayment> CreateTrafficViolationAsync(TrafficViolationDTO dto)
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

            await _unitOfWork.GetRepository<TrafficViolationPayment, int>().AddAsync(violation);
            await _unitOfWork.CompleteAsync();
            return violation;
        }

        public async Task<VehicleOwner> CreateVehicleOwnerAsync(VehicleOwnerDTO dto)
        {
            var owner = new VehicleOwner
            {
                ApplicantNID = dto.NationalId,
               // Applicant = dto.User,
                VehicleType = dto.VehicleType,
                Model = dto.Model,
                ManufactureYear = dto.ManufactureYear,
                Color = dto.Color,
                ChassisNumber = dto.ChassisNumber,
                InspectionReport = dto.InspectionReport,
                InsuranceDocument = dto.InsuranceDocument,
                OwnershipProof = dto.OwnershipProof
            };

            await _unitOfWork.GetRepository<VehicleOwner, int>().AddAsync(owner);
            await _unitOfWork.CompleteAsync();
            return owner;
        }

        public async Task<LicenseReplacementRequest> CreateLicenseReplacementAsync(LicenseReplacementDto dto)
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
            return request;
        }
    }
}