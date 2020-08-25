using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Azure.NotificationHubs;
using Scouts.Backend.Dev;

namespace Scouts.Backend.Controllers
{
    public class InstallationController : ControllerBase
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        // Initialize the Notification Hub
        NotificationHubClient _hub =
            NotificationHubClient.CreateClientFromConnectionString(Constants.NewsFullAccessConnectionString,
                Constants.NewsNotificationHubName);

        //TODO: TEST AND IMPLEMENT CLIENT-SIDE LOGIC
        [HttpPut]
        public async Task<HttpResponseMessage> CreateOrUpdateInstallation([FromBody] DeviceInstallation deviceUpdate)
        {
            var installation = new Installation
            {
                InstallationId = deviceUpdate.InstallationId,
                PushChannel = deviceUpdate.PushChannel,
                Tags = deviceUpdate.Tags,
            };

            switch (deviceUpdate.Platform)
            {
                case "Mpns":
                    installation.Platform = NotificationPlatform.Mpns;
                    break;
                case "Wns":
                    installation.Platform = NotificationPlatform.Wns;
                    break;
                case "Apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "Fcm":
                    installation.Platform = NotificationPlatform.Fcm;
                    break;
                default:
                    throw new Exception("Invalid platform request");
            }

            // In the backend we can control if a user is allowed to add tags
            //installation.Tags = new List<string>(deviceUpdate.Tags);
            //installation.Tags.Add("username:" + username);

            await _hub.CreateOrUpdateInstallationAsync(installation);

            //await CleanupInstallations();
            
            var allRegistrations = await _hub.GetAllRegistrationsAsync(0);

            foreach (var registration in allRegistrations)
            {
                var installationId = string.Empty;

                var tags = registration.Tags;
                foreach(var tag in tags)
                {
                    if (tag.Contains("InstallationId:"))
                    {
                        installationId = tag.Substring(tag.IndexOf('{'), 32);
                    }
                }

                var receivedInstallation = await _hub.GetInstallationAsync(installationId);

                if (receivedInstallation.PushChannelExpired == true) await _hub.DeleteInstallationAsync(installationId);
            }
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<DeviceInstallation> GetInstallation(string installationId)
        {
            var receivedInstallation = await _hub.GetInstallationAsync(installationId);

            return new DeviceInstallation
            {
                InstallationId = receivedInstallation.InstallationId,
                Platform = receivedInstallation.Platform.ToString(),
                PushChannel = receivedInstallation.PushChannel,
                Tags = receivedInstallation.Tags.ToList()
            };
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteInstallation(string installationId)
        {
            try
            {
                await _hub.DeleteInstallationAsync(installationId);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task CleanupInstallations()
        {
            var allRegistrations = await _hub.GetAllRegistrationsAsync(0);

            foreach (var registration in allRegistrations)
            {
                var installationId = string.Empty;

                var tags = registration.Tags;
                foreach(var tag in tags)
                {
                    if (tag.Contains("InstallationId:"))
                    {
                        installationId = tag.Substring(tag.IndexOf('{'), 32);
                    }
                }

                var receivedInstallation = await _hub.GetInstallationAsync(installationId);

                if (receivedInstallation.PushChannelExpired == true) await _hub.DeleteInstallationAsync(installationId);
            }
        }
    }
}