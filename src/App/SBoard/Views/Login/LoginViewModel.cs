using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using JetBrains.Annotations;
using ReactiveUI;
using SBoard.ApplicationModes;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Strings;
using UwCore.Application;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace SBoard.Views.Login
{
    public class LoginViewModel : ReactiveScreen
    {
        private readonly ICentronService _centronService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IApplication _application;

        private string _webServiceAddress;
        private string _username;
        private string _password;


        public string WebServiceAddress
        {
            get { return this._webServiceAddress; }
            set { this.RaiseAndSetIfChanged(ref this._webServiceAddress, value); }
        }
        public string Username
        {
            get { return this._username; }
            set { this.RaiseAndSetIfChanged(ref this._username, value); }
        }
        public string Password
        {
            get { return this._password; }
            set { this.RaiseAndSetIfChanged(ref this._password, value); }
        }

        public ReactiveCommand<Unit> Login { get; }


        public LoginViewModel([NotNull]ICentronService centronService, [NotNull]IApplicationStateService applicationStateService, [NotNull]IApplication application)
        {
            Guard.NotNull(centronService, nameof(centronService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._centronService = centronService;
            this._applicationStateService = applicationStateService;
            this._application = application;
            
            this.DisplayName = SBoardResources.Get("ViewModel.Login");

            var canLogin = this.WhenAnyValue(f => f.WebServiceAddress, f => f.Username, f => f.Password, (webServiceAddress, username, password) =>
                string.IsNullOrWhiteSpace(webServiceAddress) == false &&
                string.IsNullOrWhiteSpace(username) == false &&
                string.IsNullOrWhiteSpace(password) == false);

            this.Login = ReactiveCommand.CreateAsyncTask(canLogin, _ => this.LoginImpl());
            this.Login.AttachExceptionHandler();
            this.Login.AttachLoadingService(SBoardResources.Get("Loading.Login"));
        }


        protected override void OnActivate()
        {
            this.WebServiceAddress = this._applicationStateService.GetWebServiceAddress();
            this.Username = this._applicationStateService.GetUsername();
        }

        private async Task LoginImpl()
        {
            await this._centronService.TestLoginAsync(this.WebServiceAddress, this.Username, this.Password);

            this._applicationStateService.SetWebServiceAddress(this.WebServiceAddress);
            this._applicationStateService.SetUsername(this.Username);
            this._applicationStateService.SetPassword(this.Password);

            await this._applicationStateService.SaveStateAsync();

            this._application.CurrentMode = IoC.Get<LoggedInApplicationMode>();
        }
    }
}