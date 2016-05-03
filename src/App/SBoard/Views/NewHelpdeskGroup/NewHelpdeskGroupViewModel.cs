using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Customers;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using UwCore.Extensions;
using UwCore.Services.Loading;

namespace SBoard.Views.NewHelpdeskGroup
{
    public class NewHelpdeskGroupViewModel : ReactiveScreen
    {
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoadingService _loadingService;
        
        private string _searchText;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<CustomerPreview>> _customersHelper;
        private readonly ObservableAsPropertyHelper<bool> _isSearchingCustomersHelper;

        public string SearchText
        {
            get { return this._searchText; }
            set { this.RaiseAndSetIfChanged(ref this._searchText, value); }
        }

        public ReactiveObservableCollection<CustomerPreview> Customers
        {
            get { return this._customersHelper.Value; }
        }

        public ReactiveCommand<ReactiveObservableCollection<CustomerPreview>> SearchCustomers { get; }

        public bool IsSearchingCustomers
        {
            get { return this._isSearchingCustomersHelper.Value; }
        }

        public NewHelpdeskGroupViewModel(IHelpdeskGroupsService helpdeskGroupsService, IQueryExecutor queryExecutor, ILoadingService loadingService)
        {
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._queryExecutor = queryExecutor;
            this._loadingService = loadingService;

            var canSearchCustomers = this.WhenAnyValue(f => f.SearchText, searchText => string.IsNullOrWhiteSpace(searchText) == false);
            this.SearchCustomers = ReactiveCommand.CreateAsyncTask(canSearchCustomers, _ => this.SearchCustomersImpl(this.SearchText));
            this.SearchCustomers.ToProperty(this, f => f.Customers, out this._customersHelper);
            this.SearchCustomers.IsExecuting.ToProperty(this, f => f.IsSearchingCustomers, out this._isSearchingCustomersHelper);

            this.WhenAnyValue(f => f.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .InvokeCommand(this, f => f.SearchCustomers);
        }
        
        private async Task<ReactiveObservableCollection<CustomerPreview>> SearchCustomersImpl(string searchText)
        {
            var customers = await this._queryExecutor.ExecuteAsync(new SearchCustomersQuery(searchText));
            return new ReactiveObservableCollection<CustomerPreview>(customers);
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