using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using Bei_Daniel.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Bei_Daniel.View.Pages
{
    /// <summary>
    /// Interaction logic for ItemsOverview.xaml
    /// </summary>
    public partial class ItemsOverview : Page
    {
        private readonly int _restaurantId;

        public ItemsOverview(int restId)
        {
            InitializeComponent();
            _restaurantId = restId;

            DataContext = new OrderPageViewModel(restId);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MonthyInvoiceOverview page = new MonthyInvoiceOverview(_restaurantId);
            this.NavigationService.Navigate(page);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
