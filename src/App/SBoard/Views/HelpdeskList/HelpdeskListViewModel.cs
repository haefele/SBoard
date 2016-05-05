using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Queries;
using SBoard.Core.Queries.Helpdesks;
using SBoard.Core.Services.HelpdeskGroups;
using SBoard.Strings;
using SBoard.Views.NewHelpdeskGroup;
using UwCore.Extensions;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace SBoard.Views.HelpdeskList
{
    public class HelpdeskListViewModel : ReactiveScreen
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;
        
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<HelpdeskPreview>> _helpdesksHelper;

        private HelpdeskListKind _kind;
        private string _helpdeskGroupId;
        

        public ReactiveObservableCollection<HelpdeskPreview> Helpdesks
        {
            get { return this._helpdesksHelper.Value; }
        }
        
        public ReactiveCommand<Unit> Delete { get; }
        public ReactiveCommand<ReactiveObservableCollection<HelpdeskPreview>> RefreshHelpdesks { get; }
        
        public HelpdeskListKind Kind
        {
            get { return this._kind; }
            set { this.RaiseAndSetIfChanged(ref this._kind, value); }
        }
        public string HelpdeskGroupId
        {
            get { return this._helpdeskGroupId; }
            set { this.RaiseAndSetIfChanged(ref this._helpdeskGroupId, value); }
        }
        

        public HelpdeskListViewModel(IQueryExecutor queryExecutor, IHelpdeskGroupsService helpdeskGroupsService)
        {
            this._queryExecutor = queryExecutor;
            this._helpdeskGroupsService = helpdeskGroupsService;

            this.DisplayName = SBoardResources.Get("ViewModel.HelpdeskList");

            var canDelete = this.WhenAnyValue(f => f.Kind, kind => kind == HelpdeskListKind.HelpdeskGroup);
            this.Delete = ReactiveCommand.CreateAsyncTask(canDelete, _ => this.DeleteImpl());
            this.Delete.AttachExceptionHandler();
            this.Delete.AttachLoadingService(SBoardResources.Get("Loading.DeletingHelpdeskGroup"));

            this.RefreshHelpdesks = ReactiveCommand.CreateAsyncTask(_ => this.RefreshHelpdesksImpl());
            this.RefreshHelpdesks.AttachExceptionHandler();
            this.RefreshHelpdesks.AttachLoadingService(SBoardResources.Get("Loading.Tickets"));
            this.RefreshHelpdesks.ToProperty(this, f => f.Helpdesks, out this._helpdesksHelper);
        }

        protected override async void OnActivate()
        {
            await this.RefreshHelpdesks.ExecuteAsyncTask();
        }
        
        private async Task DeleteImpl()
        {
            await this._helpdeskGroupsService.DeleteHelpdeskGroupAsync(this.HelpdeskGroupId);
        }
        
        private async Task<ReactiveObservableCollection<HelpdeskPreview>> RefreshHelpdesksImpl()
        {
            switch (this.Kind)
            {
                case HelpdeskListKind.OnlyOwn:
                {
                    var helpdesks = await this._queryExecutor.ExecuteAsync(new OnlyOwnHelpdesksQuery());
                    return new ReactiveObservableCollection<HelpdeskPreview>(helpdesks);
                }

                case HelpdeskListKind.HelpdeskGroup:
                {
                    var helpdesks = await this._queryExecutor.ExecuteAsync(new HelpdeskGroupQuery(this.HelpdeskGroupId));
                    return new ReactiveObservableCollection<HelpdeskPreview>(helpdesks);
                }

                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}