using Bei_Daniel.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;


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
        public static string GetRestaurantNameById(long id, AppDbContext context)
        {
            var restaurant = context.Restaurants.Find(id);
            if (restaurant != null)
            {
                return restaurant.Name;
            }
            return null;
        }
        static string SanitizeForFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new System.Text.StringBuilder(input.Trim());
            for (int i = 0; i < cleaned.Length; i++)
            {
                if (Array.IndexOf(invalid, cleaned[i]) >= 0 || char.IsControl(cleaned[i]))
                    cleaned[i] = '_';
            }
            return cleaned.ToString().Replace(' ', '_');
        }
        public static string generateRestaurantFilePath(long restaurantId, AppDbContext context)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var restaurantName = RestaurantUtils.GetRestaurantNameById(restaurantId, context) ?? $"restaurant_{restaurantId}";
            var restaurantFolderName = SanitizeForFileName(restaurantName);
            var restaurantFolderPath = Path.Combine(desktopPath, "restaurants", restaurantFolderName);
            Directory.CreateDirectory(restaurantFolderPath);
            var datePart = DateTime.Now.ToString("dd_MM_yyyy");

            var baseFileName = $"Bestellung_{restaurantFolderName}_{datePart}";
            var fileName = baseFileName + ".pdf";
            var filePath = Path.Combine(restaurantFolderPath, fileName);

            int counter = 1;
            while (File.Exists(filePath))
            {
                fileName = $"{baseFileName}({counter}).pdf";
                filePath = Path.Combine(restaurantFolderPath, fileName);
                counter++;
            }

            return filePath;
        }
        public static string GetRestaurantAddressById(long id, AppDbContext context)
        {
            var restaurant = context.Restaurants.Find(id);
            if (restaurant != null)
            {
                return restaurant.Address;
            }
            return null;
        }
    }
}