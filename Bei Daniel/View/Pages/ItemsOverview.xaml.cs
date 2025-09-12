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
     private OrderPageViewModel orderPageViewModel;
        private int _restaurantId;

        public ItemsOverview(int restId)
        {
            InitializeComponent();
            _restaurantId = restId;

            orderPageViewModel = new OrderPageViewModel(restId);
            this.DataContext = orderPageViewModel;
        }


    }
}
