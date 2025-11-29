using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using Bei_Daniel.View.Pages;
using Bei_Daniel.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;


namespace Bei_Daniel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            RestaurantOverviews page = new RestaurantOverviews();
            MainFrame.Content = page;
            
        }

        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RestaurantOverviews page = new RestaurantOverviews();
            MainFrame.Content = page;
        }

        //Settings
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsOverview page = new SettingsOverview();
            MainFrame.Content = page;

        }
    }
}