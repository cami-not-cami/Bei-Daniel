using Bei_Daniel;
using Bei_Daniel.Models;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
namespace Bei_Daniel
{
    
    /// <summary>
    /// Interaction logic for RestaurantCard.xaml
    /// </summary>
    public partial class RestaurantCard : UserControl
    {
        
        public RestaurantCard()
        {
            InitializeComponent();
           
        }
      
        public static readonly DependencyProperty Navigate =
DependencyProperty.Register(nameof(NavigateCommand), typeof(ICommand), typeof(RestaurantCard));

        public ICommand NavigateCommand
        {
            get => (ICommand)GetValue(Navigate);
            set
            {
                SetValue(Navigate, value);

            }
        }
        public Restaurant Restaurant
        {
            get { return (Restaurant)GetValue(RestaurantProperty); }
            set { SetValue(RestaurantProperty, value); }
        }

        public static readonly DependencyProperty RestaurantProperty =
            DependencyProperty.Register("Restaurant", typeof(Restaurant), typeof(RestaurantCard), new PropertyMetadata(null));
    }
}

