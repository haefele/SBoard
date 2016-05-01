using Caliburn.Micro;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using UwCore.Services.ApplicationState;

namespace SBoard.Views.Login
{
    public class LoginViewModel : Screen
    {
        private readonly ICentronService _centronService;
        private readonly IApplicationStateService _applicationStateService;

        public LoginViewModel(ICentronService centronService, IApplicationStateService applicationStateService)
        {
            this._centronService = centronService;
            this._applicationStateService = applicationStateService;

            this.DisplayName = "Login";
        }

        public async void Login()
        {
            this._applicationStateService.SetWebServiceAddress(string.Empty);
            this._applicationStateService.SetUsername(string.Empty);
            this._applicationStateService.SetPassword(string.Empty);

            await this._centronService.LoginAsync();

            var tickets = await this._centronService.GetHelpdesksAsync(0);
            var priorities = await this._centronService.GetHelpdeskPrioritiesAsync();
            var types = await this._centronService.GetHelpdeskTypesAsync();
            var states = await this._centronService.GetHelpdeskStatesAsync();
            var categories = await this._centronService.GetHelpdeskCategoriesAsync();
        }
    }
}