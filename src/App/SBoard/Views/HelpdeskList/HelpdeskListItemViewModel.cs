using System;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using SBoard.Core.Data.Helpdesks;
using SBoard.Extensions;
using UwCore.Extensions;

namespace SBoard.Views.HelpdeskList
{
    public class HelpdeskListItemViewModel : ReactivePropertyChangedBase
    {
        private HelpdeskPreview _helpdesk;
        private readonly ObservableAsPropertyHelper<int> _numberHelper;
        private readonly ObservableAsPropertyHelper<string> _shortDescriptionHelper;
        private readonly ObservableAsPropertyHelper<string> _descriptionHelper;
        private readonly ObservableAsPropertyHelper<string> _priorityHelper;
        private readonly ObservableAsPropertyHelper<string> _stateHelper;
        private readonly ObservableAsPropertyHelper<TimeSpan> _plannedDurationHelper;
        private readonly ObservableAsPropertyHelper<string> _categoryHelper;
        private readonly ObservableAsPropertyHelper<string> _subCategory1Helper;
        private readonly ObservableAsPropertyHelper<string> _subCategory2Helper;

        public HelpdeskPreview Helpdesk
        {
            get { return this._helpdesk; }
            set { this.RaiseAndSetIfChanged(ref this._helpdesk, value); }
        }

        public int Number => this._numberHelper.Value;

        public string ShortDescription => this._shortDescriptionHelper.Value;

        public string Description => this._descriptionHelper.Value;

        public string Priority => this._priorityHelper.Value;

        public string Status => this._stateHelper.Value;

        public TimeSpan PlannedDuration => this._plannedDurationHelper.Value;

        public string Category => this._categoryHelper.Value;

        public string SubCategory1 => this._subCategory1Helper.Value;

        public string SubCategory2 => this._subCategory2Helper.Value;

        public HelpdeskListItemViewModel()
        {
            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.Number ?? 0)
                .ToLoadedProperty(this, f => f.Number, out this._numberHelper);
            
            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.ShortDescription ?? string.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.ShortDescription, out this._shortDescriptionHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.Description ?? string.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.Description, out this._descriptionHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.PriorityCaption ?? string.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.Priority, out this._priorityHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.StatusCaption ?? string.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.Status, out this._stateHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.PlannedDuration ?? TimeSpan.Zero)
                .ToLoadedProperty(this, f => f.PlannedDuration, out this._plannedDurationHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.CategoryCaption ?? String.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.Category, out this._categoryHelper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.SubCategory1Caption ?? String.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.SubCategory1, out this._subCategory1Helper);

            this.WhenAnyValue(f => f.Helpdesk)
                .Select(f => f?.SubCategory2Caption ?? String.Empty)
                .Select(f => f.MakeOneLiner())
                .ToLoadedProperty(this, f => f.SubCategory2, out this._subCategory2Helper);
        }
    }
}
