using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SBoard.Views.HelpdeskList
{
    public sealed partial class HelpdeskListItemView : UserControl
    {
        public HelpdeskListItemViewModel ViewModel => this.DataContext as HelpdeskListItemViewModel;

        public HelpdeskListItemView()
        {
            this.InitializeComponent();
        }
    }
}
