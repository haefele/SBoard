using System;
using Caliburn.Micro;
using JetBrains.Annotations;
using SBoard.ApplicationModes;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Strings;
using UwCore.Application;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace SBoard.Views.Login
{
    public class LoginViewModel : Screen
    {
        private readonly ICentronService _centronService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IApplication _application;

        private string _webServiceAddress;
        private string _username;
        private string _password;


        public string WebServiceAddress
        {
            get { return this._webServiceAddress; }
            set { this.SetProperty(ref this._webServiceAddress, value); }
        }

        public string Username
        {
            get { return this._username; }
            set { this.SetProperty(ref this._username, value); }
        }

        public string Password
        {
            get { return this._password; }
            set { this.SetProperty(ref this._password, value); }
        }


        public LoginViewModel([NotNull]ICentronService centronService, [NotNull]IApplicationStateService applicationStateService, [NotNull]ILoadingService loadingService, [NotNull]IExceptionHandler exceptionHandler, [NotNull]IApplication application)
        {
            Guard.NotNull(centronService, nameof(centronService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(loadingService, nameof(loadingService));
            Guard.NotNull(exceptionHandler, nameof(exceptionHandler));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._centronService = centronService;
            this._applicationStateService = applicationStateService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;
            this._application = application;
            
            this.DisplayName = SBoardResources.Get("ViewModel.Login");
        }


        protected override void OnActivate()
        {
            this.WebServiceAddress = this._applicationStateService.GetWebServiceAddress();
            this.Username = this._applicationStateService.GetUsername();
        }

        public async void Login()
        {
            using (this._loadingService.Show(SBoardResources.Get("Loading.Login")))
            {
                try
                {
                    await this._centronService.TestLoginAsync(this.WebServiceAddress, this.Username, this.Password);

                    this._applicationStateService.SetWebServiceAddress(this.WebServiceAddress);
                    this._applicationStateService.SetUsername(this.Username);
                    this._applicationStateService.SetPassword(this.Password);

                    await this._applicationStateService.SaveStateAsync();

                    this._application.CurrentMode = IoC.Get<LoggedInApplicationMode>();
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}