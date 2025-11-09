using Bei_Daniel.Helpers;
using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using Bei_Daniel.View.Pages;
using DinnerBoxd.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Bei_Daniel.ViewModel
{
    internal class MainViewModel : BaseClass
    {
        public static AppDbContext _appDbContext;
        public ICommand NavigateToItemsCommand { get; }
        public ObservableCollection<string> AvailableTypes { get; set; }

        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                    LoadRestaurants(); // <-- Call method when filter changes
                }
            }
        }

        private ObservableCollection<Restaurant> _restaurants;
        public ObservableCollection<Restaurant> Restaurants
        {
            get => _restaurants;
            set { _restaurants = value; OnPropertyChanged(nameof(Restaurants)); }
        }
        public MainViewModel()
        {
            _appDbContext = new AppDbContext();
            Restaurants = RestaurantUtils.GetRestaurants(_appDbContext);
            // Fill ComboBox with distinct types from DB
            AvailableTypes = new ObservableCollection<string>(
                _appDbContext.Restaurants.Select(r => r.Type).Distinct().ToList());
            NavigateToItemsCommand = new RelayCommand<Restaurant>(NavigateToItems);

            // Default load (first type or all)
            SelectedType = AvailableTypes.FirstOrDefault();
        }
        private void LoadRestaurants()
        {
            if (string.IsNullOrEmpty(SelectedType))
            {
                Restaurants = new ObservableCollection<Restaurant>(_appDbContext.Restaurants.ToList());
            }
            else
            {
                Restaurants = RestaurantUtils.GetRestaurantsWithFilter(_appDbContext, SelectedType);
            }
        }

        private void NavigateToItems(Restaurant restaurant)
        {

            // Get the current navigation service
            var navService = Application.Current.Windows.OfType<MainWindow>()
                             .First().MainFrame.NavigationService;

            if (navService != null)
            {
                navService.Navigate(new ItemsOverview((int)restaurant.Id));

            }
        }
    }
}
