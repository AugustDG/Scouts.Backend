using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Scouts.Backend.Hubs
{
    public class ChatHub : Hub
    {
        public async Task AddToGroup(string groupName, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Entered", userId);
        }

        public async Task RemoveFromGroup(string groupName, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Left", userId);
        }

        public async Task SendMessageGroup(string groupName, string userId, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", userId, message);
        }
    }
}