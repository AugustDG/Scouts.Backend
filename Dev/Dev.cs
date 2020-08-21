using System.Collections.Generic;

namespace Scouts.Backend.Dev
{
    public static class Constants
    {
        public static string[] SubscriptionTags { get; set; } = { "default" };
        public static string NewsNotificationHubName { get; set; } = "scouts-news";
        public static string NewsFullAccessConnectionString { get; set; } = "Endpoint=sb://scouts.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=3FhKkS9zfUTmo+Ch5WK8hsznfvxj7bVFnsgTXpFUS+A=";
    }

    public class DeviceInstallation
    {
        public string InstallationId { get; set; }
        public string Platform { get; set; }
        public string PushChannel { get; set; }
        public List<string> Tags { get; set; }
    }
}