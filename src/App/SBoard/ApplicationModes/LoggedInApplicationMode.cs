using Windows.UI.Xaml.Controls;
using SBoard.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Extensions;
using SBoard.Strings;
using SBoard.Views.HelpdeskList;
using SBoard.Views.NewHelpdeskGroup;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;
using UwCore.Services.Loading;
using UwCore.Services.Navigation;
using IoC = Caliburn.Micro.IoC;

namespace SBoard.ApplicationModes
{
    public class LoggedInApplicationMode : ApplicationMode
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;
        private readonly ICentronService _centronService;
        private readonly ILoadingService _loadingService;

        private readonly ClickableHamburgerItem _logoutItem;
        private readonly NavigatingHamburgerItem _onlyOwnItem;
        private readonly NavigatingHamburgerItem _newHelpdeskGroupItem;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, INavigationService navigationService, ICentronService centronService, ILoadingService loadingService)
        {
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;
            this._centronService = centronService;
            this._loadingService = loadingService;

            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Navigation.Logout"), SymbolEx.Logout, this.Logout);
            this._onlyOwnItem = new NavigatingHamburgerItem(SBoardResources.Get("Navigation.OwnTickets"), Symbol.List, typeof(HelpdeskListViewModel));
            this._newHelpdeskGroupItem = new NavigatingHamburgerItem(SBoardResources.Get("Navigation.NewHelpdeskGroup"), Symbol.Add, typeof(NewHelpdeskGroupViewModel));
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
            using (this._loadingService.Show(SBoardResources.Get("Loading.Logout")))
            {
                await this._centronService.LogoutAsync();

                this._applicationStateService.SetPassword(string.Empty);
                await this._applicationStateService.SaveStateAsync();

                this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
            }
        }
    }
}