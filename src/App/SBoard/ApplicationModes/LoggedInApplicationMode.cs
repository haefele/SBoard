using Windows.UI.Xaml.Controls;
using SBoard.Common;
using SBoard.Core.Services.ApplicationState;
using SBoard.Extensions;
using SBoard.Strings;
using SBoard.Views.TicketList;
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

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, INavigationService navigationService)
        {
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;

            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Logout"), SymbolEx.Logout, this.Logout);
        }

        public override void Enter()
        {   
            this.Application.SecondaryActions.Add(this._logoutItem);
            
            var ticketLists = this._applicationStateService.GetTicketLists();
            foreach (var ticketList in ticketLists)
            {
                this.Application.Actions.Add(new NavigatingHamburgerItem(ticketList.Name, Symbol.List, typeof(TicketListViewModel)));
            }

            this._navigationService.Navigate(typeof(TicketListViewModel));
        }

        public override void Leave()
        {
            this.Application.SecondaryActions.Remove(this._logoutItem);
            this.Application.Actions.RemoveWhere(f => f is NavigatingHamburgerItem && ((NavigatingHamburgerItem)f).ViewModelType == typeof(TicketListViewModel));
        }

        private async void Logout()
        {
            this._applicationStateService.SetPassword(string.Empty);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }
    }
}