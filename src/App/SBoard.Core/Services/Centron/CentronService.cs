using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Exceptions;
using SBoard.Core.Services.ApplicationState;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Services.Centron
{
    public class CentronService : ICentronService
    {
        #region Fields
        private readonly IApplicationStateService _applicationStateService;

        private string _ticket;
        #endregion

        #region Constructors
        public CentronService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;
        }
        #endregion

        #region Implementation of ICentronService
        public async Task TestLoginAsync([NotNull]string webServiceAddress, [NotNull]string username, [NotNull]string password)
        {
            Guard.NotNullOrWhiteSpace(webServiceAddress, nameof(webServiceAddress));
            Guard.NotNullOrWhiteSpace(username, nameof(username));
            Guard.NotNullOrWhiteSpace(password, nameof(password));

            await this.LoginInternalAsync(webServiceAddress, username, password);
        }

        public async Task LogoutAsync()
        {
            try
            {
                var response = await this.SendRequestAsync(this._applicationStateService.GetWebServiceAddress(), "Logout", new object());
                this._ticket = null;
            }
            catch (InvalidTicketException)
            {
                this._ticket = null;
            }
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskPreview>> GetHelpdesksAsync(int? customerI3D, bool? onlyOwn)
        {
            var request = new
            {
                Page = 1,
                EntriesPerPage = 1000,
                HelpdeskFilter = new
                {
                    CustomerI3D = customerI3D,
                    OnlyOwn = onlyOwn
                }
            };

            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetHelpdesksThroughPaging", request);

            return response
                .Values<JObject>().First()
                .Value<JArray>("Result")
                .Values<JObject>()
                .Select(this.ConvertHelpdeskPreview)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskTimer>>  GetHelpdeskTimersAsync(int helpdeskI3D)
        {
            Guard.NotZeroOrNegative(helpdeskI3D, nameof(helpdeskI3D));

            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetActiveHelpdeskTimersFromHelpdesk", helpdeskI3D);

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskTimer)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskPriority>> GetHelpdeskPrioritiesAsync()
        {
            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetActiveHelpdeskPriorities", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskPrioritiy)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskState>> GetHelpdeskStatesAsync()
        {
            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetHelpdeskStates", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskState)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskType>> GetHelpdeskTypesAsync()
        {
            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetActiveHelpdeskTypes", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertHelpdeskType)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<HelpdeskCategory>> GetHelpdeskCategoriesAsync()
        {
            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "GetActiveHelpdeskCategories", new object());

            return response
                .Values<JObject>()
                .Select(this.ConvertCategory)
                .Where(f => f != null)
                .ToList();
        }

        [ItemNotNull]
        public async Task<IList<CustomerPreview>> GetCustomersAsync(string searchText)
        {
            var request = new
            {
                Page = 1,
                EntriesPerPage = int.MaxValue,
                Filter = new
                {
                    SearchText = searchText,
                    IsActive = true
                }
            };

            var response = await this.SendAuthenticatedRequestAsync(this._applicationStateService.GetWebServiceAddress(), "SearchAccountsThroughPaging", request);

            return response
                .Values<JObject>().First()
                .Value<JArray>("Result")
                .Values<JObject>()
                .Select(this.ConvertCustomerPreview)
                .Where(f => f != null)
                .ToList();
        }
        #endregion

        #region Converter Methods
        [CanBeNull]
        private HelpdeskPreview ConvertHelpdeskPreview([CanBeNull]JObject data)
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
        [CanBeNull]
        private HelpdeskTimer ConvertHelpdeskTimer([CanBeNull]JObject data)
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
        [CanBeNull]
        private Employee ConvertEmployee([CanBeNull]JObject data)
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
        [CanBeNull]
        private HelpdeskPriority ConvertHelpdeskPrioritiy([CanBeNull]JObject data)
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
        [CanBeNull]
        private HelpdeskState ConvertHelpdeskState([CanBeNull]JObject data)
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
        [CanBeNull]
        private HelpdeskType ConvertHelpdeskType([CanBeNull]JObject data)
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
        [CanBeNull]
        private HelpdeskCategory ConvertCategory([CanBeNull]JObject data)
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
                    .Where(f => f != null)
                    .ToList()
            };
        }
        [CanBeNull]
        private CustomerPreview ConvertCustomerPreview(JObject data)
        {
            if (data == null)
                return null;

            return new CustomerPreview
            {
                I3D = data.Value<int>("I3D"),
                Name = data.Value<string>("AccountName"),

                Original = data
            };
        }
        #endregion

        #region Private Methods
        [ItemNotNull]
        private async Task<string> LoginInternalAsync([NotNull]string address, [NotNull]string username, [NotNull]string password)
        {
            Guard.NotNullOrWhiteSpace(address, nameof(address));
            Guard.NotNullOrWhiteSpace(username, nameof(username));
            Guard.NotNullOrWhiteSpace(password, nameof(password));

            var request = new
            {
                LoginKind = 0,
                Username = username,
                Password = password,
                Device = new EasClientDeviceInformation().FriendlyName,
                Application = "{8A9E6F82-4DFF-4C93-916C-CEA90EE4ED22}",
                AppVersion = Package.Current.Id.Version.ToVersion().ToString(),
            };

            var response = await this.SendRequestAsync(address, "Login", request);

            return response.Values<string>().First();
        }

        [ItemNotNull]
        private async Task<JArray> SendAuthenticatedRequestAsync([NotNull]string address, [NotNull]string method, [NotNull]object request)
        {
            Guard.NotNullOrWhiteSpace(address, nameof(address));
            Guard.NotNullOrWhiteSpace(method, nameof(method));
            Guard.NotNull(request, nameof(request));

            try
            {
                return await this.SendRequestAsync(address, method, request);
            }
            catch (InvalidTicketException)
            {
                this._ticket = await this.LoginInternalAsync(address, this._applicationStateService.GetUsername(), this._applicationStateService.GetPassword());
                return await this.SendAuthenticatedRequestAsync(address, method, request);
            }
        }

        [ItemNotNull]
        private async Task<JArray> SendRequestAsync([NotNull]string address, [NotNull]string method, [NotNull]object request)
        {
            Guard.NotNullOrWhiteSpace(address, nameof(address));
            Guard.NotNullOrWhiteSpace(method, nameof(method));
            Guard.NotNull(request, nameof(request));

            var requestJObject = new JObject
            {
                ["Ticket"] = this._ticket,
                ["Data"] = JToken.FromObject(request)
            };

            var json = JsonConvert.SerializeObject(requestJObject);
            var response = await this.GetClient(address).PostAsync(method, new StringContent(json, Encoding.UTF8, "application/json"));

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJObject = JObject.Parse(responseString);

            if (responseJObject.Value<int>("Status") == 2)
                throw new InvalidTicketException();

            if (responseJObject.Value<int>("Status") != 0)
                throw new SBoardException(responseJObject.Value<string>("Message"));

            return responseJObject.Value<JArray>("Result") ?? new JArray();
        }

        [NotNull]
        private HttpClient GetClient([NotNull]string address)
        {
            Guard.NotNullOrWhiteSpace(address, nameof(address));

            address = this.SanitizeWebServiceAddress(address);

            return new HttpClient
            {
                BaseAddress = new Uri(address)
            };
        }

        [NotNull]
        private string SanitizeWebServiceAddress([NotNull]string address)
        {
            Guard.NotNullOrWhiteSpace(address, nameof(address));

            string[] endings =
            {
                "REST",
                "SOAP",
                "HELP",
                "RESTC",
                "/"
            };

            while (endings.Any(f => address.EndsWith(f, StringComparison.OrdinalIgnoreCase)))
            {
                var matchingEnding = endings.First(f => address.EndsWith(f, StringComparison.CurrentCultureIgnoreCase));

                address = address.Substring(0, address.Length - matchingEnding.Length);
            }

            return address + "/REST/";
        }
        #endregion
    }
}