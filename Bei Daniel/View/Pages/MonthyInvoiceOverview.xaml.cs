using Bei_Daniel.Utils;
using Bei_Daniel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bei_Daniel.View.Pages
{
    /// <summary>
    /// Interaction logic for MonthyInvoiceOverview.xaml
    /// </summary>
    public partial class MonthyInvoiceOverview : Page
    {
        private readonly int _restaurantId;
        public MonthyInvoiceOverview(int restaurantId)
        {
            InitializeComponent();
            _restaurantId= restaurantId;
            DataContext = new MonthyInvoiceViewModel(restaurantId);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MonthyInvoiceViewModel viewModel)
            {
                // The StartDate and EndDate properties in the viewModel are already
                // bound to the DatePickers in XAML, so they will have the correct values
                viewModel.PrintIntervalSelectedInvoice(_restaurantId);
            }
        }
    }
}
