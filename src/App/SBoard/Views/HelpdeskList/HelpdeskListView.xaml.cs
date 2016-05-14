using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveUI;
using SBoard.Behaviors;
using System.Reactive.Linq;
using Caliburn.Micro;

namespace SBoard.Views.HelpdeskList
{
    public sealed partial class HelpdeskListView : Page
    {
        private MenuFlyout _menu;

        public HelpdeskListViewModel ViewModel => this.DataContext as HelpdeskListViewModel;

        public HelpdeskListView()
        {
            this.InitializeComponent();
        }

        public void ShowContextMenu(object sender, ContextMenuTriggerBehaviorData data)
        {
            if (this._menu == null)
                this._menu = this.CreateMenu();

            if (data.Data is HelpdeskListItemViewModel && this._menu != null)
            {
                this.ViewModel.SelectedHelpdesk = (HelpdeskListItemViewModel)data.Data;
                this._menu.ShowAt(data.Control, data.Position);
            }
        }
        
        private MenuFlyout CreateMenu()
        {
            var menu = new MenuFlyout();
            var changeStateItem = new MenuFlyoutSubItem
            {
                Text = "Status ändern",
            };
            menu.Items.Add(changeStateItem);

            foreach (var state in this.ViewModel.States)
            {
                var stateItem = new MenuFlyoutItem
                {
                    Text = state.Name,
                };
                changeStateItem.Items.Add(stateItem);

                stateItem.Click += (_, __) =>
                {
                    this.ViewModel.SelectedState = state;
                    this.ViewModel.ChangeState.ExecuteAsyncTask();
                };
            }

            return menu;
        }
    }
}
