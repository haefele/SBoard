using Windows.UI.Xaml;
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

        private void ClearSelectedHelpdeskType(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SelectedHelpdeskType = null;
        }

        private void ClearSelectedHelpdeskState(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SelectedHelpdeskState = null;
        }
    }
}
