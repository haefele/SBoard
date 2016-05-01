using System;
using System.Collections.Generic;
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

        #region Implementation of ICentronService
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

            this._ticket = response.Values<string>().First();
        }

        public async Task LogoutAsync()
        {
            var response = await this.SendRequestAsync("Logout", new object());
            this._ticket = null;
        }

        public async Task<IList<HelpdeskPreview>> GetHelpdesksAsync(int customerI3D)
        {
            var request = new
            {
                Page = 1,
                EntriesPerPage = 1000,
                HelpdeskFilter = new
                {
                    CustomerI3D = customerI3D
                }
            };

            var response = await this.SendRequestAsync("GetHelpdesksThroughPaging", request);

            return response
                .Values<JObject>().First()
                .Value<JArray>("Result")
                .Values<JObject>()
                .Select(this.ConvertHelpdeskPreview)
                .ToList();
        }

        public async Task<IList<HelpdeskTimer>>  GetHelpdeskTimersAsync(int helpdeskI3D)
        {
            var response = await this.SendRequestAsync("GetActiveHelpdeskTimersFromHelpdesk", helpdeskI3D);

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskTimer)
                .ToList();
        }

        public async Task<IList<HelpdeskPriority>> GetHelpdeskPrioritiesAsync()
        {
            var response = await this.SendRequestAsync("GetActiveHelpdeskPriorities", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskPrioritiy)
                .ToList();
        }

        public async Task<IList<HelpdeskState>> GetHelpdeskStatesAsync()
        {
            var response = await this.SendRequestAsync("GetHelpdeskStates", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskState)
                .ToList();
        }

        public async Task<IList<HelpdeskType>> GetHelpdeskTypesAsync()
        {
            var response = await this.SendRequestAsync("GetActiveHelpdeskTypes", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskType)
                .ToList();
        }

        public async Task<IList<HelpdeskCategory>> GetHelpdeskCategoriesAsync()
        {
            var response = await this.SendRequestAsync("GetActiveHelpdeskCategories", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertCategory)
                .ToList();
        }
        #endregion

        #region Converter Methods
        private HelpdeskPreview ConvertHelpdeskPreview(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskPreview
            {
                I3D = data.Value<int>("I3D"),

                Number = data.Value<int>("Number"),

                Description = data.Value<string>("Description"),
                ShortDescription = data.Value<string>("ShortDescription"),
                InternalNote = data.Value<string>("InternalNote"),

                PlannedDuration = TimeSpan.FromHours(data.Value<double?>("PlannedDurationInHours") ?? 0),
                DueDate = data.Value<DateTime?>("DueDate"),

                ResponsiblePerson = this.ConvertEmployee(data.Value<JObject>("ResponsiblePerson")),
                Editors = data.Value<JArray>("Editors")
                    .Values<JObject>()
                    .Select(this.ConvertEmployee)
                    .ToList(),

                PriorityI3D = data.Value<int?>("HelpdeskPriorityI3D"),
                PriorityCaption = data.Value<string>("HelpdeskPriorityCaption"),

                StatusI3D = data.Value<int?>("HelpdeskStatusI3D"),
                StatusCaption = data.Value<string>("HelpdeskStatusCaption"),

                TypeI3D = data.Value<int?>("HelpdeskTypeI3D"),
                TypeCaption = data.Value<string>("HelpdeskTypeCaption"),

                CategoryI3D = data.Value<int?>("MainCategoryI3D"),
                CategoryCaption = data.Value<string>("MainCategoryCaption"),

                SubCategory1I3D = data.Value<int?>("SubCategory1I3D"),
                SubCategory1Caption = data.Value<string>("SubCategory1Caption"),

                SubCategory2I3D = data.Value<int?>("SubCategory2I3D"),
                SubCategory2Caption = data.Value<string>("SubCategory2Caption"),

                IsCalculable = data.Value<bool>("IsCalculable"),

                Original = data
            };
        }
        private HelpdeskTimer ConvertHelpdeskTimer(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskTimer
            {
                I3D = data.Value<int>("I3D"),
                IsCalculable = data.Value<bool>("Calculable"),
                Description = data.Value<string>("ExternalNote"),
                InternalDescription = data.Value<string>("InternalNote"),
                IsPlanned = data.Value<bool>("IsPlanned"),
                LunchTime = TimeSpan.FromSeconds(data.Value<int>("LunchTime")),
                Start = data.Value<DateTime>("Start"),
                End = data.Value<DateTime>("Stop"),

                Original = data
            };
        }
        private Employee ConvertEmployee(JObject data)
        {
            if (data == null)
                return null;

            return new Employee
            {
                I3D = data.Value<int>("I3D"),
                EmailAddress = data.Value<string>("Email"),
                FirstName = data.Value<string>("FirstName"),
                LastName = data.Value<string>("LastName"),
                ShortSign = data.Value<string>("ShortSign")
            };
        }
        private HelpdeskPriority ConvertHelpdeskPrioritiy(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskPriority
            {
                I3D = data.Value<int>("I3D"),
                Name = data.Value<string>("Name"),
                IsDeactivated = data.Value<bool?>("IsDeactivated") ?? false,

                Original = data,
            };
        }
        private HelpdeskState ConvertHelpdeskState(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskState
            {
                I3D = data.Value<int>("I3D"),
                Name = data.Value<string>("Name"),
                IsDeactivated = data.Value<bool?>("IsDeactivated") ?? false,

                Original = data
            };
        }
        private HelpdeskType ConvertHelpdeskType(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskType
            {
                I3D = data.Value<int>("I3D"),
                Name = data.Value<string>("Name"),
                IsDeactivated = data.Value<bool?>("IsDeactivated") ?? false,

                Original = data
            };
        }
        private HelpdeskCategory ConvertCategory(JObject data)
        {
            if (data == null)
                return null;

            return new HelpdeskCategory
            {
                I3D = data.Value<int>("I3D"),
                Name = data.Value<string>("Name"),
                IsDeactivated = data.Value<bool?>("IsDeactivated") ?? false,
                SubCategories = data
                    .Value<JArray>("SubCategories")
                    .Values<JObject>()
                    .Select(this.ConvertCategory)
                    .ToList()
            };
        }
        #endregion

        #region Private Methods
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
        #endregion
    }
}