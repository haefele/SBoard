using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Helpdesks;
using SBoard.Strings;
using UwCore.Extensions;
using UwCore.Services.Loading;

namespace SBoard.Views.HelpdeskList
{
    public class HelpdeskListViewModel : Screen
    {
        #region Fields
        private readonly ILoadingService _loadingService;
        private readonly IQueryExecutor _queryExecutor;

        private BindableCollection<HelpdeskPreview> _helpdesks;
        #endregion

        #region Properties
        public BindableCollection<HelpdeskPreview> Helpdesks
        {
            get { return this._helpdesks; }
            set { this.SetProperty(ref this._helpdesks, value); }
        }
        #endregion

        #region Parameter
        public HelpdeskListKind Kind { get; set; }
        public string HelpdeskGroupId { get; set; }
        #endregion

        #region Constructors
        public HelpdeskListViewModel(ILoadingService loadingService, IQueryExecutor queryExecutor)
        {
            this._loadingService = loadingService;
            this._queryExecutor = queryExecutor;

            this.DisplayName = SBoardResources.Get("ViewModel.HelpdeskList");

            this.Kind = HelpdeskListKind.OnlyOwn;
        }
        #endregion

        protected override async void OnActivate()
        {
            await this.RefreshInternalAsync();
        }

        public new async void Refresh()
        {
            await this.RefreshInternalAsync();
        }

        private async Task RefreshInternalAsync()
        {
            using (this._loadingService.Show(SBoardResources.Get("Loading.Tickets")))
            {
                switch (this.Kind)
                {
                    case HelpdeskListKind.OnlyOwn:
                    {
                        var helpdesks = await this._queryExecutor.ExecuteAsync(new OnlyOwnHelpdesksQuery());
                        this.Helpdesks = new BindableCollection<HelpdeskPreview>(helpdesks);
                    }
                    break;
                        
                    case HelpdeskListKind.HelpdeskGroup:
                    {
                        var helpdesks = await this._queryExecutor.ExecuteAsync(new HelpdeskGroupQuery(this.HelpdeskGroupId));
                        this.Helpdesks = new BindableCollection<HelpdeskPreview>(helpdesks);
                    }
                    break;

                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}