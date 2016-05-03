using Windows.UI.Xaml.Controls;
using SBoard.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Extensions;
using SBoard.Strings;
using SBoard.Views.HelpdeskList;
using SBoard.Views.NewHelpdeskGroup;
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
        private readonly NavigatingHamburgerItem _newHelpdeskGroupItem;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, INavigationService navigationService)
        {
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;
            
            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Logout"), SymbolEx.Logout, this.Logout);
            this._onlyOwnItem = new NavigatingHamburgerItem(SBoardResources.Get("OwnTickets"), Symbol.List, typeof(HelpdeskListViewModel));
            this._newHelpdeskGroupItem = new NavigatingHamburgerItem("Neue Ticket-Gruppe", Symbol.Add, typeof(NewHelpdeskGroupViewModel));
        }

        public override void Enter()
        {   
            this.Application.SecondaryActions.Add(this._logoutItem);
            this.Application.Actions.Add(this._onlyOwnItem);
            this.Application.Actions.Add(this._newHelpdeskGroupItem);

            this._navigationService.For<HelpdeskListViewModel>()
                .WithParam(f => f.Kind, HelpdeskListKind.OnlyOwn)
                .Navigate();
        }

        public override void Leave()
        {
            this.Application.SecondaryActions.Remove(this._logoutItem);
            this.Application.Actions.Remove(this._onlyOwnItem);
            this.Application.Actions.Remove(this._newHelpdeskGroupItem);
        }

        private async void Logout()
        {
            this._applicationStateService.SetPassword(string.Empty);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }
    }
}