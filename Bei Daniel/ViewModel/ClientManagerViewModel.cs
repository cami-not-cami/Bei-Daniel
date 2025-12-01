using Bei_Daniel.Helpers;
using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using DinnerBoxd.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Bei_Daniel;
using Bei_Daniel.View.Pages;

namespace Bei_Daniel.ViewModel
{
    internal class ClientManagerViewModel : BaseClass
    {
        private AppDbContext _context = new AppDbContext();
        private long? _restaurantId;
        private bool _isEditMode;

        // Properties for the form
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _address = string.Empty;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _selectedType = "Restaurant";
        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> RestaurantTypes { get; set; }

        private string _pageTitle = "Neues Restaurant erstellen";
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged(nameof(PageTitle));
            }
        }

        private string _saveButtonText = "Restaurant Erstellen";
        public string SaveButtonText
        {
            get { return _saveButtonText; }
            set
            {
                _saveButtonText = value;
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        // Commands
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        // Constructor for creating new restaurant
        public ClientManagerViewModel()
        {
            InitializeViewModel();
            _isEditMode = false;
            PageTitle = "Neues Restaurant erstellen";
            SaveButtonText = "Restaurant Erstellen";
        }

        // Constructor for editing existing restaurant
        public ClientManagerViewModel(int restaurantId)
        {
            InitializeViewModel();
            _restaurantId = restaurantId;
            _isEditMode = true;
            PageTitle = "Restaurant bearbeiten";
            SaveButtonText = "Änderungen Speichern";
            LoadRestaurant(restaurantId);
        }

        private void InitializeViewModel()
        {
            RestaurantTypes = RestaurantUtils.RESTAURANT_TYPES;

            SaveCommand = new RelayCommand(
                () => SaveRestaurant(null),
                () => CanSave(null)
            );
            CancelCommand = new RelayCommand(NavigateBack);
        }

        private void LoadRestaurant(int restaurantId)
        {
            var restaurant = RestaurantUtils.GetRestaurantById(restaurantId, _context);
            if (restaurant != null)
            {
                Name = restaurant.Name;
                Address = restaurant.Address;
                SelectedType = restaurant.Type;
            }
        }

        private bool CanSave(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   !string.IsNullOrWhiteSpace(SelectedType);
        }

        private void SaveRestaurant(object parameter)
        {
            if (_isEditMode && _restaurantId.HasValue)
            {
                // Update existing restaurant
                RestaurantUtils.UpdateRestaurant(_restaurantId.Value, _context, Name, Address, SelectedType);
                MessageBox.Show("Restaurant erfolgreich aktualisiert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Create new restaurant
                RestaurantUtils.CreateRestaurant(_context, Name, Address, SelectedType);
                MessageBox.Show("Restaurant erfolgreich erstellt!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            NavigateBack();
        }

        private void NavigateBack()
        {
            if (Application.Current?.MainWindow is MainWindow mainWindow)
            {
                if (_isEditMode && _restaurantId.HasValue)
                {
                    mainWindow.MainFrame.Navigate(new ItemsOverview((int)_restaurantId.Value));
                }
                else
                {
                    mainWindow.MainFrame.Content = new RestaurantOverviews();
                }
            }
        }
    }
}