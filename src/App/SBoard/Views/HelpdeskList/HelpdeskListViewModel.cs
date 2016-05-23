using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Helpdesks;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Strings;
using SBoard.Views.Dashboard;
using UwCore.Extensions;
using UwCore.Services.Loading;
using INavigationService = UwCore.Services.Navigation.INavigationService;
using SBoard.Core.Commands;
using SBoard.Core.Commands.Helpdesks;
using SBoard.Extensions;

namespace SBoard.Views.HelpdeskList
{
    public class HelpdeskListViewModel : ReactiveScreen
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly INavigationService _navigationService;
        private readonly ICommandQueue _commandQueue;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<HelpdeskListItemViewModel>> _helpdesksHelper;
        private HelpdeskListItemViewModel _selectedHelpdesk;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<HelpdeskState>> _statesHelper;
        private HelpdeskState _selectedState;
        
        private string _helpdeskGroupId;


        public ReactiveObservableCollection<HelpdeskListItemViewModel> Helpdesks
        {
            get { return this._helpdesksHelper.Value; }
        }
        public HelpdeskListItemViewModel SelectedHelpdesk
        {
            get { return this._selectedHelpdesk; }
            set { this.RaiseAndSetIfChanged(ref this._selectedHelpdesk, value); }
        }
        public ReactiveObservableCollection<HelpdeskState> States
        {
            get { return this._statesHelper.Value; }
        }
        public HelpdeskState SelectedState
        {
            get { return this._selectedState; }
            set { this.RaiseAndSetIfChanged(ref this._selectedState, value); }
        }


        public ReactiveCommand<Unit> Delete { get; }
        public ReactiveCommand<ReactiveObservableCollection<HelpdeskPreview>> RefreshHelpdesks { get; }
        public ReactiveCommand<ReactiveObservableCollection<HelpdeskState>> LoadStates { get; }
        public ReactiveCommand<Unit> ChangeState { get; }
        
        public string HelpdeskGroupId
        {
            get { return this._helpdeskGroupId; }
            set { this.RaiseAndSetIfChanged(ref this._helpdeskGroupId, value); }
        }
        

        public HelpdeskListViewModel(IQueryExecutor queryExecutor, IHelpdeskGroupsService helpdeskGroupsService, INavigationService navigationService, ICommandQueue commandQueue)
        {
            this._queryExecutor = queryExecutor;
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._navigationService = navigationService;
            this._commandQueue = commandQueue;

            this.DisplayName = SBoardResources.Get("ViewModel.HelpdeskList");
            
            this.Delete = ReactiveCommand.CreateAsyncTask(_ => this.DeleteImpl());
            this.Delete.AttachExceptionHandler();
            this.Delete.AttachLoadingService(SBoardResources.Get("Loading.DeletingHelpdeskGroup"));

            this.RefreshHelpdesks = ReactiveCommand.CreateAsyncTask(_ => this.RefreshHelpdesksImpl());
            this.RefreshHelpdesks.AttachExceptionHandler();
            this.RefreshHelpdesks.AttachLoadingService(SBoardResources.Get("Loading.Tickets"));
            this.RefreshHelpdesks
                .Select(f =>
                {
                    var result = new ReactiveObservableCollection<HelpdeskListItemViewModel>();

                    foreach (var helpdesk in f.OrderByDescending(d => d.I3D))
                    {
                        var viewModel = IoC.Get<HelpdeskListItemViewModel>();
                        viewModel.Helpdesk = helpdesk;

                        result.Add(viewModel);
                    }

                    return result;
                })
                .ToLoadedProperty(this, f => f.Helpdesks, out this._helpdesksHelper);
            
            this.LoadStates = ReactiveCommand.CreateAsyncTask(_ => this.LoadStatesImpl());
            this.LoadStates.AttachExceptionHandler();
            this.LoadStates.AttachLoadingService(SBoardResources.Get("Loading.TicketStates"));
            this.LoadStates.ToLoadedProperty(this, f => f.States, out this._statesHelper);

            var canChangeState = this.WhenAnyValue(f => f.SelectedState, f => f.SelectedHelpdesk, (state, helpdesk) => state != null && helpdesk != null);
            this.ChangeState = ReactiveCommand.CreateAsyncTask(canChangeState, _ => this.ChangeStateImpl());
            this.ChangeState.AttachExceptionHandler();
            this.ChangeState.AttachLoadingService(SBoardResources.Get("Loading.ChangingTicketState"));
        }

        protected override async void OnActivate()
        {
            await Task.WhenAll(
                this.RefreshHelpdesks.ExecuteAsyncTask(),
                this.LoadStates.ExecuteAsyncTask());
        }
        
        private async Task DeleteImpl()
        {
            await this._helpdeskGroupsService.DeleteHelpdeskGroupAsync(this.HelpdeskGroupId);

            this._navigationService.For<DashboardViewModel>().Navigate();
            this._navigationService.ClearBackStack();
        }
        
        private async Task<ReactiveObservableCollection<HelpdeskPreview>> RefreshHelpdesksImpl()
        {
            var queryResult = await this._queryExecutor.ExecuteAsync(new HelpdeskGroupQuery(this.HelpdeskGroupId));
            return new ReactiveObservableCollection<HelpdeskPreview>(queryResult.Result);
        }

        private async Task<ReactiveObservableCollection<HelpdeskState>> LoadStatesImpl()
        {
            var queryResult = await this._queryExecutor.ExecuteAsync(new HelpdeskStatesQuery(onlyActive: true));
            return new ReactiveObservableCollection<HelpdeskState>(queryResult.Result);
        }

        private async Task ChangeStateImpl()
        {
            var command = new ChangeHelpdeskStateCommand(this.SelectedHelpdesk.Helpdesk.I3D, this.SelectedState.I3D);
            await this._commandQueue.EnqueueAsync(command);
        }
    }
}