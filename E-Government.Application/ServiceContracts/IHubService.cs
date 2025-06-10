using E_Government.Application.DTO.AdminDashboard;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface IHubService
    {
        // Method name changed to match AdminController usage:
        Task SendAdminNotification(string message);

        // Other client methods as previously discussed and aligned with frontend guide:
        Task ReceiveAdminBroadcast(string message);
        Task ReceiveRequestUpdated(RequestSummaryDto updatedRequest);
        Task ReceiveStatisticsUpdate(DashboardStatisticsDto stats);
        Task ReceiveNewRequest(RequestSummaryDto newRequest);
        Task ReceiveRequestDeleted(string requestId); // If you implement request deletion notifications
        Task ReceiveChartUpdate(string chartName, ChartDataDto chartData);
        Task ReceiveUserListUpdate(PagedResult<UserSummaryDto> users); // If you implement user list updates

        // If your existing methods like SendRequestNotification, SendRequestStatusUpdate 
        // from your original IHubService were intended to be messages sent TO clients, 
        // they should be renamed to "Receive..." and included above.
        // For example, if AdminController needs to tell clients about a specific request status update:
        Task ReceiveSpecificRequestStatusUpdate(string userId, string requestType, string requestId, string status);
    }
}
