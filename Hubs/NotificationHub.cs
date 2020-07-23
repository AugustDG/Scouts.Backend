using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.NotificationHubs;
using Scouts.Backend.Dev;

namespace Scouts.Backend.Hubs
{
    public class NotificationsHub : Hub
    {
        public async Task SendNotificationUser(string msg, string destUser)
        {
            NotificationHubClient hub =
                NotificationHubClient.CreateClientFromConnectionString(Constants.NewsFullAccessConnectionString,
                    Constants.NewsNotificationHubName);
            Notification notification = new FcmNotification("{\"data\":{\"message\":\"" + msg + "\"}}");

            await hub.SendNotificationAsync(notification, destUser);
        }
    }
}