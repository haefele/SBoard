using System;
using System.Collections.Generic;
using Caliburn.Micro;
using SBoard.ApplicationModes;
using SBoard.Core.Common;
using SBoard.Core.Services.Centron;
using SBoard.Strings;
using SBoard.Views.Login;
using UwCore.Application;

namespace SBoard
{
    sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        public override ApplicationMode GetCurrentMode()
        {
            return IoC.Get<LoggedOutApplicationMode>();
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
    }
}
