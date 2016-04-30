using Windows.UI.Xaml.Controls;

namespace SBoard.Views.Login
{
    public sealed partial class LoginView : Page
    {
        public LoginViewModel ViewModel => this.DataContext as LoginViewModel;

        public LoginView()
        {
            this.InitializeComponent();
        }
    }
}
