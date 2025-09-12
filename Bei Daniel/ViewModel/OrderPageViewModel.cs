using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Bei_Daniel.ViewModel
{
    class OrderPageViewModel
    {
        private int _restaurantId;
        private readonly AppDbContext _appDbContext = MainViewModel._appDbContext;
        public ObservableCollection<Order> Orders { get; set; }
        public OrderPageViewModel(int restaurantId) 
        {

            // Load items for this restaurant using the ID
            _restaurantId = restaurantId;
            LoadItemsForRestaurant(_restaurantId);
         
        }
        private void LoadItemsForRestaurant(long restaurantId)
        {
            var orders = _appDbContext.Orders
     .Include(o => o.Product)
     .Where(o => o.RestaurantId == restaurantId)
     .ToList();

            Orders = new ObservableCollection<Order>(orders);
        }
    }
}
