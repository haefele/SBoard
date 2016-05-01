using Windows.UI.Xaml.Controls;
using SBoard.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Extensions;
using SBoard.Strings;
using SBoard.Views.HelpdeskList;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;
using UwCore.Services.Navigation;
using IoC = Caliburn.Micro.IoC;

namespace SBoard.ApplicationModes
{
    public class LoggedInApplicationMode : ApplicationMode
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;

        private readonly ClickableHamburgerItem _logoutItem;
        private readonly NavigatingHamburgerItem _onlyOwnItem;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, INavigationService navigationService)
        {
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;
            
            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Logout"), SymbolEx.Logout, this.Logout);
            this._onlyOwnItem = new NavigatingHamburgerItem(SBoardResources.Get("OwnTickets"), Symbol.List, typeof(HelpdeskListViewModel));
        }

        public override void Enter()
        {   
            this.Application.SecondaryActions.Add(this._logoutItem);
            this.Application.Actions.Add(this._onlyOwnItem);

            this._navigationService.Navigate(typeof(HelpdeskListViewModel));
        }

        public override void Leave()
        {
            this.Application.SecondaryActions.Remove(this._logoutItem);
            this.Application.Actions.Remove(this._onlyOwnItem);
        }

        private async void Logout()
        {
            this._applicationStateService.SetPassword(string.Empty);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }
    }
}