using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.HelpdeskGroups;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Strings;
using UwCore.Extensions;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace SBoard.Views.NewHelpdeskGroup
{
    public class NewHelpdeskGroupViewModel : ReactiveScreen
    {
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

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

        public NewHelpdeskGroupViewModel(IHelpdeskGroupsService helpdeskGroupsService, IQueryExecutor queryExecutor, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._queryExecutor = queryExecutor;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = SBoardResources.Get("ViewModel.NewHelpdeskGroup");
            
            this.SearchCustomers = ReactiveCommand.CreateAsyncTask(_ => this.SearchCustomersImpl());
            this.SearchCustomers.ToProperty(this, f => f.Customers, out this._customersHelper);
            this.SearchCustomers.IsExecuting.ToProperty(this, f => f.IsSearchingCustomers, out this._isSearchingCustomersHelper);
            this.SearchCustomers.ThrownExceptions.Subscribe(async e => await this._exceptionHandler.HandleAsync(e));
            
            this.WhenAnyValue(f => f.CustomerSearchText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .InvokeCommand(this, f => f.SearchCustomers);

            var canSave = this.WhenAnyValue(f => f.Name, f => f.SelectedCustomer, (name, selectedCustomer) =>
                string.IsNullOrWhiteSpace(name) == false && selectedCustomer != null);
            this.Save = ReactiveCommand.CreateAsyncTask(canSave, _ => this.SaveImpl());
            this.Save.AttachLoadingService(this._loadingService, SBoardResources.Get("Loading.Saving"));
            this.Save.ThrownExceptions.Subscribe(async e => await this._exceptionHandler.HandleAsync(e));
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
            await this._helpdeskGroupsService.AddHelpdeskGroupAsync(this._name, new WebServiceHelpdeskFilter {CustomerI3D = this.SelectedCustomer.I3D}, null);
            this.TryClose(true);
        }
    }

    public static class Extensions
    {
        public static void AttachLoadingService(this IReactiveCommand self, ILoadingService loadingService, string message)
        {
            IDisposable disposable = null;
            self.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    disposable = loadingService.Show(message);
                }
                else
                {
                    disposable?.Dispose();
                }
            });
        }
    }
}