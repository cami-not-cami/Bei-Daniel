using Bei_Daniel.ViewModel;
using System.Windows.Controls;

namespace Bei_Daniel.View.Pages
{
    public partial class SettingsOverview : Page
    {
        public SettingsOverview()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}
