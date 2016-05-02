using Windows.UI.Xaml.Controls;

namespace SBoard.Views.NewHelpdeskGroup
{
    public sealed partial class NewHelpdeskGroupView : Page
    {
        public NewHelpdeskGroupViewModel ViewModel => this.DataContext as NewHelpdeskGroupViewModel;

        public NewHelpdeskGroupView()
        {
            this.InitializeComponent();
        }
    }
}
