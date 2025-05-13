using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
   public interface IHubService
    {
        Task SendRequestNotification(string userId, string requestType, string requestId);
        Task SendRequestStatusUpdate(string userId, string requestType, string requestId, string status);
        Task SendAdminNotification(string message);
    }
}
