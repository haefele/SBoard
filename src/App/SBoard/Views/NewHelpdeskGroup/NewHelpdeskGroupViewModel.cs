using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.HelpdeskGroups;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
using SBoard.Core.Queries.Helpdesks;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Strings;
using SBoard.Views.HelpdeskList;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.Navigation;

namespace SBoard.Views.NewHelpdeskGroup
{
    public class NewHelpdeskGroupViewModel : ReactiveScreen
    {
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly INavigationService _navigationService;

        private string _name;
        private string _customerSearchText;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<CustomerPreview>> _customersHelper;
        private CustomerPreview _selectedCustomer;
        private readonly ObservableAsPropertyHelper<bool> _isSearchingCustomersHelper;
        private bool _onlyOwnTickets;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<HelpdeskType>> _helpdeskTypesHelper;
        private HelpdeskType _selectedHelpdeskType;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<HelpdeskState>> _helpdeskStatesHelper;
        private HelpdeskState _selectedHelpdeskState;

        public string Name
        {
            get { return this._name; }
            set { this.RaiseAndSetIfChanged(ref this._name, value); }
        }
        public string CustomerSearchText
        {
            get { return this._customerSearchText; }
            set { this.RaiseAndSetIfChanged(ref this._customerSearchText, value); }
        }
        public ReactiveObservableCollection<CustomerPreview> Customers
        {
            get { return this._customersHelper.Value; }
        }
        public CustomerPreview SelectedCustomer
        {
            get { return this._selectedCustomer; }
            set { this.RaiseAndSetIfChanged(ref this._selectedCustomer, value); }
        }
        public bool IsSearchingCustomers
        {
            get { return this._isSearchingCustomersHelper.Value; }
        }
        public bool OnlyOwnTickets
        {
            get { return this._onlyOwnTickets; }
            set { this.RaiseAndSetIfChanged(ref this._onlyOwnTickets, value); }
        }
        public ReactiveObservableCollection<HelpdeskType> HelpdeskTypes
        {
            get { return this._helpdeskTypesHelper.Value; }
        }
        public HelpdeskType SelectedHelpdeskType
        {
            get { return this._selectedHelpdeskType; }
            set { this.RaiseAndSetIfChanged(ref this._selectedHelpdeskType, value); }
        }
        public ReactiveObservableCollection<HelpdeskState> HelpdeskStates
        {
            get { return this._helpdeskStatesHelper.Value; }
        }
        public HelpdeskState SelectedHelpdeskState
        {
            get { return this._selectedHelpdeskState; }
            set { this.RaiseAndSetIfChanged(ref this._selectedHelpdeskState, value); }
        }


        public ReactiveCommand<ReactiveObservableCollection<HelpdeskType>> LoadHelpdeskTypes { get; }
        public ReactiveCommand<ReactiveObservableCollection<HelpdeskState>> LoadHelpdeskStates { get; }
        public ReactiveCommand<ReactiveObservableCollection<CustomerPreview>> SearchCustomers { get; }
        public ReactiveCommand<Unit> Save { get; }


        public NewHelpdeskGroupViewModel(IHelpdeskGroupsService helpdeskGroupsService, IQueryExecutor queryExecutor, INavigationService navigationService)
        {
            Guard.NotNull(helpdeskGroupsService, nameof(helpdeskGroupsService));
            Guard.NotNull(queryExecutor, nameof(queryExecutor));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._helpdeskGroupsService = helpdeskGroupsService;
            this._queryExecutor = queryExecutor;
            this._navigationService = navigationService;

            this.DisplayName = SBoardResources.Get("ViewModel.NewHelpdeskGroup");

            this.LoadHelpdeskTypes = ReactiveCommand.CreateAsyncTask(_ => this.LoadHelpdeskTypesImpl());
            this.LoadHelpdeskTypes.ToProperty(this, f => f.HelpdeskTypes, out this._helpdeskTypesHelper);
            this.LoadHelpdeskTypes.AttachExceptionHandler();
            this.LoadHelpdeskTypes.AttachLoadingService(SBoardResources.Get("Loading.TicketTypes"));

            this.LoadHelpdeskStates = ReactiveCommand.CreateAsyncTask(_ => this.LoadHelpdeskStatesImpl());
            this.LoadHelpdeskStates.ToProperty(this, f => f.HelpdeskStates, out this._helpdeskStatesHelper);
            this.LoadHelpdeskStates.AttachExceptionHandler();
            this.LoadHelpdeskStates.AttachLoadingService(SBoardResources.Get("Loading.TicketStates"));

            this.SearchCustomers = ReactiveCommand.CreateAsyncTask(_ => this.SearchCustomersImpl());
            this.SearchCustomers.ToProperty(this, f => f.Customers, out this._customersHelper);
            this.SearchCustomers.IsExecuting.ToProperty(this, f => f.IsSearchingCustomers, out this._isSearchingCustomersHelper);
            this.SearchCustomers.AttachExceptionHandler();
            
            this.WhenAnyValue(f => f.CustomerSearchText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .InvokeCommand(this, f => f.SearchCustomers);

            var canSave = this.WhenAnyValue(f => f.Name, name => string.IsNullOrWhiteSpace(name) == false);
            this.Save = ReactiveCommand.CreateAsyncTask(canSave, _ => this.SaveImpl());
            this.Save.AttachLoadingService(SBoardResources.Get("Loading.Saving"));
            this.Save.AttachExceptionHandler();
        }


        protected override async void OnInitialize()
        {
            await this.LoadHelpdeskTypes.ExecuteAsyncTask();
            await this.LoadHelpdeskStates.ExecuteAsyncTask();
        }


        private async Task<ReactiveObservableCollection<HelpdeskType>> LoadHelpdeskTypesImpl()
        {
            var queryResult = await this._queryExecutor.ExecuteAsync(new HelpdeskTypesQuery(onlyActive:true));
            return new ReactiveObservableCollection<HelpdeskType>(queryResult.Result);
        }

        private async Task<ReactiveObservableCollection<HelpdeskState>> LoadHelpdeskStatesImpl()
        {
            var queryResult = await this._queryExecutor.ExecuteAsync(new HelpdeskStatesQuery(onlyActive: true));
            return new ReactiveObservableCollection<HelpdeskState>(queryResult.Result);
        }

        private async Task<ReactiveObservableCollection<CustomerPreview>> SearchCustomersImpl()
        {
            if (string.IsNullOrWhiteSpace(this.CustomerSearchText))
                return new ReactiveObservableCollection<CustomerPreview>();

            var queryResult = await this._queryExecutor.ExecuteAsync(new SearchCustomersQuery(this.CustomerSearchText));
            return new ReactiveObservableCollection<CustomerPreview>(queryResult.Result);
        }

        private async Task SaveImpl()
        {
            var group = await this._helpdeskGroupsService.AddHelpdeskGroupAsync(
                this._name, 
                this.SelectedCustomer?.I3D,
                this.OnlyOwnTickets,
                this.SelectedHelpdeskType?.I3D,
                this.SelectedHelpdeskState?.I3D);
            
            this._navigationService.For<HelpdeskListViewModel>()
                .WithParam(f => f.HelpdeskGroupId, group.Id)
                .Navigate();

            this._navigationService.ClearBackStack();
        }
    }
}