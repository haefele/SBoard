using Windows.UI.Xaml.Controls;

namespace SBoard.Views.HelpdeskList
{
    public sealed partial class HelpdeskListView : Page
    {
        public HelpdeskListViewModel ViewModel => this.DataContext as HelpdeskListViewModel;

        public HelpdeskListView()
        {
            this.InitializeComponent();
        }
    }
}
