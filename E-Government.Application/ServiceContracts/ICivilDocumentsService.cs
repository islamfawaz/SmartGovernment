using E_Government.Application.DTO.CivilDocs;
using E_Government.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface ICivilDocumentsService
    {
        public Task<Guid> SubmitRequestAsync(CivilDocumentRequestDto dto);

        public Task<CivilDocumentRequestDto> GetRequestStatusAsync(Guid requestId);
        public Task<List<CivilDocumentRequestDto>> GetUserRequestsAsync(string userNID);

        public Task AddAttachmentAsync(Guid requestId, string fileType, string filePath);

        public Task UpdateRequestStatusAsync(Guid requestId, string newStatus, string note);

    }
}
