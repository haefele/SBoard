using Windows.UI.Xaml.Controls;
using UwCore.Behaviors;

namespace SBoard.Views.HelpdeskList
{
    public sealed partial class HelpdeskListView : Page
    {
        public HelpdeskListViewModel ViewModel => this.DataContext as HelpdeskListViewModel;

        public HelpdeskListView()
        {
            this.InitializeComponent();
        }
        
        private void MenuFlyoutBehavior_OnCreateMenu(object sender, CreateMenuEventArgs e)
        {
            e.Menu = new MenuFlyout();
            var changeStateItem = new MenuFlyoutSubItem
            {
                Text = "Status ändern",
            };
            e.Menu.Items.Add(changeStateItem);

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
        }

        private void MenuFlyoutBehavior_OnMenuShowing(object sender, MenuShowingEventArgs e)
        {
            if (e.Data is HelpdeskListItemViewModel)
            {
                this.ViewModel.SelectedHelpdesk = (HelpdeskListItemViewModel) e.Data;
            }
        }
    }
}
