using Caliburn.Micro.ReactiveUI;
using SBoard.Strings;

namespace SBoard.Views.Dashboard
{
    public class DashboardViewModel : ReactiveScreen
    {
        public DashboardViewModel()
        {
            this.DisplayName = SBoardResources.Get("ViewModel.Dashboard");
        }
    }
}