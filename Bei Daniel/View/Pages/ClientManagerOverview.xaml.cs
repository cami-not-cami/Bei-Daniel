using Bei_Daniel.ViewModel;
using System.Windows.Controls;

namespace Bei_Daniel.View.Pages
{
    /// <summary>
    /// Interaction logic for ClientManagerOverview.xaml
    /// </summary>
    public partial class ClientManagerOverview : Page
    {
        public ClientManagerOverview()
        {
            InitializeComponent();
            DataContext = new ClientManagerViewModel();
        }

        public ClientManagerOverview(int restaurantId)
        {
            InitializeComponent();
            DataContext = new ClientManagerViewModel(restaurantId);
        }
    }
}