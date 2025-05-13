using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace E_Government.Application.Services
{
    public class CivilDocumentsService : ICivilDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CivilDocumentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> SubmitRequestAsync(CivilDocumentRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.DocumentType))
                throw new ArgumentException("Document type is required", nameof(dto.DocumentType));
            if (string.IsNullOrWhiteSpace(dto.ApplicantName))
                throw new ArgumentException("Applicant name is required", nameof(dto.ApplicantName));
            if (string.IsNullOrWhiteSpace(dto.ApplicantNID))
                throw new ArgumentException("Applicant NID is required", nameof(dto.ApplicantNID));
            if (string.IsNullOrWhiteSpace(dto.OwnerName))
                throw new ArgumentException("Owner name is required", nameof(dto.OwnerName));
            if (string.IsNullOrWhiteSpace(dto.OwnerNID))
                throw new ArgumentException("Owner NID is required", nameof(dto.OwnerNID));
            if (string.IsNullOrWhiteSpace(dto.OwnerMotherName))
                throw new ArgumentException("Owner mother name is required", nameof(dto.OwnerMotherName));
            if (dto.CopiesCount <= 0)
                throw new ArgumentException("Copies count must be greater than 0", nameof(dto.CopiesCount));

            // Trim NID values
            var trimmedApplicantNID = dto.ApplicantNID.Trim();
            var trimmedOwnerNID = dto.OwnerNID.Trim();

            // Validate user exists
            var user = await _unitOfWork.GetRepository<ApplicationUser, string>()
                .GetAsync(trimmedApplicantNID);

            if (user == null)
            {
                throw new InvalidOperationException($"User with NID {trimmedApplicantNID} does not exist.");
            }

            var requestId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var entity = new CivilDocumentRequest
            {
                Id = requestId,
                DocumentType = dto.DocumentType,
                ApplicantName = dto.ApplicantName,
                ApplicantNID = trimmedApplicantNID,
                Relation = dto.Relation,
                OwnerName = dto.OwnerName,
                OwnerNID = trimmedOwnerNID,
                OwnerMotherName = dto.OwnerMotherName,
                CopiesCount = dto.CopiesCount,
                Status = "New",
                CreatedAt = now,
                LastUpdated = now,
                ExtraFieldsJson = dto.ExtraFields != null ? 
                    System.Text.Json.JsonSerializer.Serialize(dto.ExtraFields) : "{}"
            };

            // Create initial history record
            var history = new CivilDocumentRequestHistory
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                Status = "New",
                Note = "Request created",
                ChangedAt = now
            };

            await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().AddAsync(entity);
            await _unitOfWork.GetRepository<CivilDocumentRequestHistory, Guid>().AddAsync(history);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<CivilDocumentRequestDto> GetRequestStatusAsync(Guid requestId)
        {
            var entity = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetAllWithIncludeAsync(q => q
                    .Where(r => r.Id == requestId)
                    .Include(r => r.Attachments)
                    .Include(r => r.History))
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (entity == null) return null;

            return new CivilDocumentRequestDto
            {
                Id = entity.Id,
                DocumentType = entity.DocumentType,
                ApplicantName = entity.ApplicantName,
                ApplicantNID = entity.ApplicantNID?.Trim(),
                Relation = entity.Relation,
                OwnerName = entity.OwnerName,
                OwnerNID = entity.OwnerNID?.Trim(),
                OwnerMotherName = entity.OwnerMotherName,
                CopiesCount = entity.CopiesCount,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                Attachments = entity.Attachments?.Select(a => new CivilDocumentAttachmentDto
                {
                    Id = a.Id,
                    FileType = a.FileType,
                    FilePath = a.FilePath,
                    UploadedAt = a.UploadedAt
                }).ToList() ?? new List<CivilDocumentAttachmentDto>(),
                History = entity.History?.Select(h => new CivilDocumentRequestHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status,
                    Note = h.Note,
                    ChangedAt = h.ChangedAt
                }).ToList() ?? new List<CivilDocumentRequestHistoryDto>(),
                ExtraFields = !string.IsNullOrEmpty(entity.ExtraFieldsJson) ? 
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(entity.ExtraFieldsJson) : 
                    new Dictionary<string, string>()
            };
        }

        public async Task<List<CivilDocumentRequestDto>> GetUserRequestsAsync(string userNID)
        {
            var trimmedUserNID = userNID?.Trim();
            var requests = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetAllWithIncludeAsync(q => q
                    .Where(r => r.ApplicantNID == trimmedUserNID)
                    .Include(r => r.Attachments)
                    .Include(r => r.History));

            return requests.Select(r => new CivilDocumentRequestDto
            {
                Id = r.Id,
                DocumentType = r.DocumentType,
                ApplicantName = r.ApplicantName,
                ApplicantNID = r.ApplicantNID?.Trim(),
                Relation = r.Relation,
                OwnerName = r.OwnerName,
                OwnerNID = r.OwnerNID?.Trim(),
                OwnerMotherName = r.OwnerMotherName,
                CopiesCount = r.CopiesCount,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                Attachments = r.Attachments?.Select(a => new CivilDocumentAttachmentDto
                {
                    Id = a.Id,
                    FileType = a.FileType,
                    FilePath = a.FilePath,
                    UploadedAt = a.UploadedAt
                }).ToList() ?? new List<CivilDocumentAttachmentDto>(),
                History = r.History?.Select(h => new CivilDocumentRequestHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status,
                    Note = h.Note,
                    ChangedAt = h.ChangedAt
                }).ToList() ?? new List<CivilDocumentRequestHistoryDto>(),
                ExtraFields = !string.IsNullOrEmpty(r.ExtraFieldsJson) ? 
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(r.ExtraFieldsJson) : 
                    new Dictionary<string, string>()
            }).ToList();
        }

        public async Task AddAttachmentAsync(Guid requestId, string fileType, string filePath)
        {
            var attachment = new CivilDocumentAttachment
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                FileType = fileType,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<CivilDocumentAttachment, Guid>().AddAsync(attachment);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateRequestStatusAsync(Guid requestId, string newStatus, string note)
        {
            var request = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetAsync(requestId);

            if (request == null)
                throw new InvalidOperationException($"Request with ID {requestId} not found.");

            var now = DateTime.UtcNow;

            // Create history record
            var history = new CivilDocumentRequestHistory
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                Status = newStatus,
                Note = note,
                ChangedAt = now
            };

            // Update request
            request.Status = newStatus;
            request.LastUpdated = now;

            await _unitOfWork.GetRepository<CivilDocumentRequestHistory, Guid>().AddAsync(history);
            await _unitOfWork.CompleteAsync();
        }
    }
} 