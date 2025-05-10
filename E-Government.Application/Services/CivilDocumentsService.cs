using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;

namespace E_Government.Application.Services
{
    public class CivilDocumentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CivilDocumentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> SubmitRequestAsync(CivilDocumentRequestDto dto)
        {
            var entity = new CivilDocumentRequest
            {
                Id = Guid.NewGuid(),
                DocumentType = dto.DocumentType,
                ApplicantName = dto.ApplicantName,
                ApplicantNID = dto.ApplicantNID,
                Relation = dto.Relation,
                OwnerName = dto.OwnerName,
                OwnerNID = dto.OwnerNID,
                OwnerMotherName = dto.OwnerMotherName,
                CopiesCount = dto.CopiesCount,
                Status = "New",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<CivilDocumentRequestDto> GetRequestStatusAsync(Guid requestId)
        {
            var entity = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().GetAsync(requestId);
            if (entity == null) return null;
            return new CivilDocumentRequestDto
            {
                Id = entity.Id,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<List<CivilDocumentRequestDto>> GetUserRequestsAsync(string userNID)
        {
            var requests = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetAllWithIncludeAsync(q => q.Where(r => r.ApplicantNID == userNID));
            return requests.Select(r => new CivilDocumentRequestDto
            {
                Id = r.Id,
                DocumentType = r.DocumentType,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        // Placeholder for adding attachments
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
    }
} 