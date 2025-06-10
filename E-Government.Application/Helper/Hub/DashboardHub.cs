using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities;
using E_Government.Domain.ServiceContracts;
using Microsoft.AspNetCore.SignalR;

namespace E_Government.Domain.Helper.Hub
{
    public class DashboardHub : Hub<IHubService> // Typed Hub using IHubService for client methods
    {
        // Server-side methods that can be called by clients (if any needed for admin dashboard)
        // For example, an admin might send a broadcast message through the hub.
        public async Task SendBroadcastMessage(string message)
        {
            // This would call the "ReceiveAdminBroadcast" method on ALL connected clients that implement IHubService
            await Clients.All.ReceiveAdminBroadcast(message);
        }

        public async Task NotifyRequestUpdate(RequestSummaryDto updatedRequest)
        {
            // This method is intended to be called by the server (e.g., AdminController) 
            // to push updates to clients, not directly by clients.
            // The AdminController will use IHubContext<DashboardHub, IHubService>.
            // However, if an admin client *itself* wanted to trigger this for other admins (less common):
            await Clients.Others.ReceiveRequestUpdated(updatedRequest); // Send to other clients
        }

        // It's more common for the Hub to be a recipient of calls from the server-side logic (e.g. AdminController)
        // which then broadcasts to clients. The methods defined in IHubService are what the *server* calls on *clients*.

        // Example of a client-callable method (less likely for this specific dashboard scenario from admin client)
        // public async Task AdminAction(string actionDetails)
        // {
        //     // Process admin action and potentially notify other clients
        //     await Clients.All.ReceiveAdminNotification($"Admin action performed: {actionDetails}");
        // }

        public override async Task OnConnectedAsync()
        {
            // Example: Add user to a group, log connection
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception? exception)
        {
            // Example: Remove user from a group, log disconnection
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
            await base.OnDisconnectedAsync(exception);
        }
    }

}
