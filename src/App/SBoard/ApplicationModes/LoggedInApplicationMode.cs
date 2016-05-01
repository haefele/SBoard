using Caliburn.Micro;
using SBoard.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Strings;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;

namespace SBoard.ApplicationModes
{
    public class LoggedInApplicationMode : ApplicationMode
    {
        private readonly IApplicationStateService _applicationStateService;

        private readonly ClickableHamburgerItem _logoutItem;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService)
        {
            this._applicationStateService = applicationStateService;

            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Logout"), SymbolEx.Logout, this.Logout);
        }

        public override void Enter()
        {   
            this.Application.SecondaryActions.Add(this._logoutItem);
        }

        public override void Leave()
        {
            this.Application.SecondaryActions.Remove(this._logoutItem);
        }

        private async void Logout()
        {
            this._applicationStateService.SetPassword(string.Empty);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }
    }
}