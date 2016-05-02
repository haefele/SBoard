using System.Threading.Tasks;
using Caliburn.Micro;
using SBoard.Core.Data.Customers;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Customers;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;
using UwCore.Extensions;
using UwCore.Services.Loading;

namespace SBoard.Views.NewHelpdeskGroup
{
    public class NewHelpdeskGroupViewModel : Screen
    {
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ILoadingService _loadingService;

        private BindableCollection<CustomerPreview> _customers;

        public BindableCollection<CustomerPreview> Customers
        {
            get { return this._customers; }
            set { this.SetProperty(ref this._customers, value); }
        }

        public NewHelpdeskGroupViewModel(IHelpdeskGroupsService helpdeskGroupsService, IQueryExecutor queryExecutor, ILoadingService loadingService)
        {
            this._helpdeskGroupsService = helpdeskGroupsService;
            this._queryExecutor = queryExecutor;
            this._loadingService = loadingService;
        }

        protected override async void OnActivate()
        {
            await this.RefreshInternalAsync();
        }

        private async Task RefreshInternalAsync()
        {
            using (this._loadingService.Show("Lade Daten..."))
            {
                var customers = await this._queryExecutor.ExecuteAsync(new AllCustomersQuery());
                this.Customers = new BindableCollection<CustomerPreview>(customers);
            }
        }
    }
}