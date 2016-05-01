using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Caliburn.Micro;
using SBoard.ApplicationModes;
using SBoard.Core.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Strings;
using SBoard.Views.Login;
using UwCore.Application;
using UwCore.Services.ApplicationState;

namespace SBoard
{
    sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            base.Configure();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }

        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(LoginViewModel);
        }

        public override void ConfigureContainer(WinRTContainer container)
        {
            container
                .PerRequest<LoggedOutApplicationMode>()
                .PerRequest<LoggedInApplicationMode>();

            container.Singleton<ICentronService, CentronService>();
        }

        public override string GetErrorTitle() => SBoardResources.Get("Errors.Title");

        public override string GetErrorMessage() => SBoardResources.Get("Errors.Message");

        public override Type GetCommonExceptionType() => typeof(SBoardException);
        
        public override ApplicationMode GetCurrentMode()
        {
            var state = IoC.Get<IApplicationStateService>();
            string webServiceAddress = state.GetWebServiceAddress();
            string username = state.GetUsername();
            string password = state.GetPassword();

            if (string.IsNullOrWhiteSpace(webServiceAddress) == false &&
                string.IsNullOrWhiteSpace(username) == false &&
                string.IsNullOrWhiteSpace(password) == false)
            {
                return IoC.Get<LoggedInApplicationMode>();
            }
            else
            {
                return IoC.Get<LoggedOutApplicationMode>();
            }
        }
    }
}
