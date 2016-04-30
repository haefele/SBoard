using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBoard.Core.Common;
using SBoard.Core.Services.ApplicationState;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Services.Centron
{
    public class CentronService : ICentronService
    {
        private readonly IApplicationStateService _applicationStateService;
        private string _ticket;

        public CentronService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;
        }

        public async Task LoginAsync()
        {
            var request = new
            {
                LoginKind = 0,
                Username = this._applicationStateService.GetUsername(),
                Password = this._applicationStateService.GetPassword(),
                Device = string.Empty,
                Application = string.Empty,
                AppVersion = Package.Current.Id.Version.ToVersion().ToString(),
            };

            var response = await this.SendRequestAsync("Login", request);
        }

        private async Task<JArray> SendRequestAsync(string method, object request)
        {
            var requestJObject = new JObject
            {
                ["Ticket"] = this._ticket,
                ["Data"] = JToken.FromObject(request)
            };

            var json = JsonConvert.SerializeObject(requestJObject);
            var response = await this.GetClient().PostAsync(method, new StringContent(json, Encoding.UTF8, "application/json"));

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJObject = JObject.Parse(responseString);

            if (responseJObject.Value<int>("Status") != 0)
                throw new SBoardException(responseJObject.Value<string>("Message"));

            return responseJObject.Value<JArray>("Result");
        }

        private HttpClient GetClient()
        {
            var webServiceAddress = this._applicationStateService.GetWebServiceAddress();
            webServiceAddress = this.SanitizeWebServiceAddress(webServiceAddress);

            return new HttpClient
            {
                BaseAddress = new Uri(webServiceAddress)
            };
        }

        private string SanitizeWebServiceAddress(string webServiceAddress)
        {
            string[] endings =
            {
                "REST",
                "SOAP",
                "HELP",
                "RESTC",
                "/"
            };

            while (endings.Any(f => webServiceAddress.EndsWith(f, StringComparison.OrdinalIgnoreCase)))
            {
                var matchingEnding = endings.First(f => webServiceAddress.EndsWith(f, StringComparison.CurrentCultureIgnoreCase));

                webServiceAddress = webServiceAddress.Substring(0, webServiceAddress.Length - matchingEnding.Length);
            }

            return webServiceAddress + "/REST/";
        }
    }
}