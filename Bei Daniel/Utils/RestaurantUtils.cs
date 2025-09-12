using Bei_Daniel.Models;
using System.Collections.ObjectModel;


namespace Bei_Daniel.Utils
{
    internal class RestaurantUtils
    {
        public static ObservableCollection<Restaurant> GetRestaurants(AppDbContext context)
        {
            ObservableCollection<Restaurant> restaurants = new ObservableCollection<Restaurant>();

            foreach (var item in context.Restaurants.ToList())
            {
                restaurants.Add(item);
            }
            return restaurants;
        }
        public static ObservableCollection<Restaurant> GetRestaurantsWithFilter(AppDbContext context, string filter)
        {
            ObservableCollection<Restaurant> restaurants = new ObservableCollection<Restaurant>();

            foreach (var item in context.Restaurants.Where(r => r.Type == filter).ToList())
            {
                restaurants.Add(item);
            }
            return restaurants;
        }
        public static void AddRestaurant(Restaurant restaurant, AppDbContext context)
        {
            context.Restaurants.Add(restaurant);
            context.SaveChangesAsync();
        }
        public static void RemoveRestaurantById(long id, AppDbContext context)
        {
            var restaurant = context.Restaurants.Find(id);
            if (restaurant != null)
            {
                context.Restaurants.Remove(restaurant);
                context.SaveChanges();
            }
        }
        public static void SolveResturantOrders(long restaurantId, AppDbContext context)
        {
            var orders = OrderUtils.GetAllUnsolvedOrders(restaurantId, context);
            foreach (var order in orders)
            {
                order.Solved = true;
            }
            context.SaveChanges();
        }
    }
}