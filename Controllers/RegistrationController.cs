using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Scouts.Backend.Dev;

namespace Scouts.Backend.Controllers
{
    public class RegistrationController : ControllerBase
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

        [HttpGet]
        public async Task<RegistrationDescription> RegistrateDevice(string platform, string pnsId, string[] tags)
        {
            RegistrationDescription reg = null;

            // create a registration description object of the correct type, e.g.
            switch (platform)
            {
                case "fcm":
                    reg = new FcmRegistrationDescription(pnsId, tags);
                    break;
            }

            var registeredDesc = await _hub.CreateRegistrationAsync(reg);

            return registeredDesc;
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteRegistration(string platform, string pnsId)
        {
            RegistrationDescription reg = null;

            // create a registration description object of the correct type, e.g.
            switch (platform)
            {
                case "fcm":
                    reg = new FcmRegistrationDescription(pnsId);
                    break;
            }

            if (reg is null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
            {
                await _hub.DeleteRegistrationsByChannelAsync(reg.PnsHandle);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        [HttpGet]
        public async Task<RegistrationDescription> AddUpdateRegistration(string regId, string[] tagsToAdd)
        {
            var reg = await _hub.GetRegistrationAsync<RegistrationDescription>(regId);

            foreach (var tag in tagsToAdd)
            {
                var hasAdded = reg.Tags.Add(tag);

                if (!hasAdded) throw new Exception();
            }

            var newReg = await _hub.CreateOrUpdateRegistrationAsync(reg);

            return newReg;
        }

        [HttpGet]
        public async Task<RegistrationDescription> RemoveUpdateRegistration(string regId, string[] tagsToRemove)
        {
            var reg = await _hub.GetRegistrationAsync<RegistrationDescription>(regId);

            foreach (var tag in tagsToRemove)
            {
                var hasAdded = reg.Tags.Remove(tag);

                if (!hasAdded) throw new Exception();
            }

            var newReg = await _hub.CreateOrUpdateRegistrationAsync(reg);

            return newReg;
        }
    }
}