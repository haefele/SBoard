using Windows.UI.Xaml.Controls;

namespace SBoard.Views.TicketList
{
    public sealed partial class TicketListView : Page
    {
        public TicketListViewModel ViewModel => this.DataContext as TicketListViewModel;

        public TicketListView()
        {
            this.InitializeComponent();
        }
    }
}
