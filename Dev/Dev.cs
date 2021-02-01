using System.Collections.Generic;

namespace Scouts.Backend.Dev
{
    public static class Constants
    {
        public static string[] SubscriptionTags { get; set; } = { "default" };
        public static string NewsNotificationHubName { get; set; } = "scouts-news";
        public static string NewsFullAccessConnectionString { get; set; } = "{CONNECTION_STRING}";
    }

    public class DeviceInstallation
    {
        public string InstallationId { get; set; }
        public string Platform { get; set; }
        public string PushChannel { get; set; }
        public List<string> Tags { get; set; }
        public long ExpirationTime { get; set; }
    }
}
