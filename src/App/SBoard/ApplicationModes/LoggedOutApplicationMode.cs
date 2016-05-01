using Windows.UI.Xaml.Controls;
using SBoard.Common;
using SBoard.Strings;
using SBoard.Views.Login;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.Navigation;

namespace SBoard.ApplicationModes
{
    public class LoggedOutApplicationMode : ApplicationMode
    {
        private readonly INavigationService _navigationService;
        private readonly HamburgerItem _loginItem;

        public LoggedOutApplicationMode(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this._loginItem = new NavigatingHamburgerItem(SBoardResources.Get("ViewModel.Login"), SymbolEx.Login, typeof(LoginViewModel));
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._loginItem);

            this._navigationService.Navigate(typeof(LoginViewModel));
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._loginItem);
        }
    }
}