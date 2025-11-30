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
    /// Interaction logic for ClientManagerOverview.xaml
    /// </summary>
    public partial class ClientManagerOverview : Page
    {
        public ClientManagerOverview()
        {
            InitializeComponent();
            DataContext = new ClientManagerViewModel();
        }
    }
}
