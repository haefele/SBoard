using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.HelpdeskGroups;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
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
            
            this.SearchCustomers = ReactiveCommand.CreateAsyncTask(_ => this.SearchCustomersImpl());
            this.SearchCustomers.ToProperty(this, f => f.Customers, out this._customersHelper);
            this.SearchCustomers.IsExecuting.ToProperty(this, f => f.IsSearchingCustomers, out this._isSearchingCustomersHelper);
            this.SearchCustomers.AttachExceptionHandler();
            
            this.WhenAnyValue(f => f.CustomerSearchText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .InvokeCommand(this, f => f.SearchCustomers);

            var canSave = this.WhenAnyValue(f => f.Name, f => f.SelectedCustomer, (name, selectedCustomer) =>
                string.IsNullOrWhiteSpace(name) == false && selectedCustomer != null);
            this.Save = ReactiveCommand.CreateAsyncTask(canSave, _ => this.SaveImpl());
            this.Save.AttachLoadingService(SBoardResources.Get("Loading.Saving"));
            this.Save.AttachExceptionHandler();
        }
        

        private async Task<ReactiveObservableCollection<CustomerPreview>> SearchCustomersImpl()
        {
            if (string.IsNullOrWhiteSpace(this.CustomerSearchText))
                return new ReactiveObservableCollection<CustomerPreview>();

            var customers = await this._queryExecutor.ExecuteAsync(new SearchCustomersQuery(this.CustomerSearchText));
            return new ReactiveObservableCollection<CustomerPreview>(customers);
        }

        private async Task SaveImpl()
        {
            var group = await this._helpdeskGroupsService.AddHelpdeskGroupAsync(
                this._name, 
                new WebServiceHelpdeskFilter {CustomerI3D = this.SelectedCustomer.I3D}, 
                null);
            
            this._navigationService.For<HelpdeskListViewModel>()
                .WithParam(f => f.Kind, HelpdeskListKind.HelpdeskGroup)
                .WithParam(f => f.HelpdeskGroupId, group.Id)
                .Navigate();

            this._navigationService.ClearBackStack();
        }
    }
}