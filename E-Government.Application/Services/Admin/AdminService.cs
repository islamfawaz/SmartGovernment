using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services.Admin
{
    public  class AdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllUser()
        {
            var user= await _unitOfWork.GetRepository<ApplicationUser,string>().GetAllAsync();

            if (user is null)
            {
                throw new NotFoundException("No User Avaliable");
            }
           
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(user);
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
                ApplicantNID = entity.ApplicantNID?.Trim()!,
                Relation = entity.Relation,
                OwnerName = entity.OwnerName,
                OwnerNID = entity.OwnerNID?.Trim()!,
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
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(entity.ExtraFieldsJson)! :
                    new Dictionary<string, string>()
            };
        }

    }
}
