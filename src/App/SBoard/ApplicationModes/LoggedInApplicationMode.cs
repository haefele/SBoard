using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using SBoard.Common;
using SBoard.Core.Data.HelpdeskGroups;
using SBoard.Core.Services.ApplicationState;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Core.Services.HelpdeskGroups.Events;
using SBoard.Strings;
using SBoard.Views.Dashboard;
using SBoard.Views.HelpdeskList;
using SBoard.Views.NewHelpdeskGroup;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;
using UwCore.Services.Loading;

namespace SBoard.ApplicationModes
{
    public class LoggedInApplicationMode : ApplicationMode, IHandle<HelpdeskGroupAdded>, IHandle<HelpdeskGroupDeleted>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICentronService _centronService;
        private readonly ILoadingService _loadingService;
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IEventAggregator _eventAggregator;

        private readonly NavigatingHamburgerItem _dashboardItem;
        private readonly ClickableHamburgerItem _logoutItem;
        private readonly NavigatingHamburgerItem _newHelpdeskGroupItem;
        private readonly IList<NavigatingHamburgerItem> _helpdeskGroupItems;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, ICentronService centronService, ILoadingService loadingService, IHelpdeskGroupsService helpdeskGroupsService, IEventAggregator eventAggregator)
        {
            this._applicationStateService = applicationStateService;
            this._centronService = centronService;
            this._loadingService = loadingService;
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._eventAggregator = eventAggregator;

            this._dashboardItem = new NavigatingHamburgerItem(SBoardResources.Get("Navigation.Dashboard"), Symbol.Home, typeof(DashboardViewModel));
            this._logoutItem = new ClickableHamburgerItem(SBoardResources.Get("Navigation.Logout"), SymbolEx.Logout, this.Logout);
            this._newHelpdeskGroupItem = new NavigatingHamburgerItem(SBoardResources.Get("Navigation.NewHelpdeskGroup"), Symbol.Add, typeof(NewHelpdeskGroupViewModel));
            this._helpdeskGroupItems = new List<NavigatingHamburgerItem>();
        }

        public override async void Enter()
        {   
            this._eventAggregator.Subscribe(this);

            this.Application.Actions.Add(this._dashboardItem);
            this.Application.SecondaryActions.Add(this._logoutItem);
            this.Application.Actions.Add(this._newHelpdeskGroupItem);

            this._helpdeskGroupItems.Clear();

            var groups = await this._helpdeskGroupsService.GetHelpdeskGroupsAsync();
            foreach (var group in groups)
            {
                this.AddHelpdeskGroupItem(group);
            }

            this._dashboardItem.Execute();
        }

        public override void Leave()
        {
            this._eventAggregator.Unsubscribe(this);

            this.Application.Actions.Remove(this._dashboardItem);
            this.Application.SecondaryActions.Remove(this._logoutItem);
            this.Application.Actions.Remove(this._newHelpdeskGroupItem);

            foreach (var item in this._helpdeskGroupItems)
            {
                this.Application.Actions.Remove(item);
            }

            this._helpdeskGroupItems.Clear();
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

        void IHandle<HelpdeskGroupAdded>.Handle(HelpdeskGroupAdded message)
        {
            this.AddHelpdeskGroupItem(message.HelpdeskGroup);
        }

        private void AddHelpdeskGroupItem(HelpdeskGroup group)
        {
            var item = new NavigatingHamburgerItem(group.Name, Symbol.List, typeof(HelpdeskListViewModel));
            item.AddParameter<HelpdeskListViewModel>(f => f.HelpdeskGroupId, group.Id);

            this._helpdeskGroupItems.Add(item);
            this.Application.Actions.Add(item);
        }

        void IHandle<HelpdeskGroupDeleted>.Handle(HelpdeskGroupDeleted message)
        {
            var item = this._helpdeskGroupItems.FirstOrDefault(f => 
                f.ViewModelType == typeof(HelpdeskListViewModel) &&
                f.TryGetParameterValue<string>(nameof(HelpdeskListViewModel.HelpdeskGroupId)) == message.HelpdeskGroup.Id);

            if (item != null)
            {
                this.Application.Actions.Remove(item);
            }
        }
    }
}