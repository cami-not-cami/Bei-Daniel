using Bei_Daniel.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace Bei_Daniel.Utils
{
    internal class OrderUtils
    {
        private static AppDbContext context = new AppDbContext();
        public static ObservableCollection<Order> GetOrders(long restaurantId)
        {
            ObservableCollection<Order> order = new ObservableCollection<Order>();

            foreach (var item in context.Orders.Include(x => x.Product).Where(o => o.RestaurantId == restaurantId).ToList())
            {
                order.Add(item);
            }
            return order;
        }

        public static ObservableCollection<Order> GetOrders(long restaurantId, DateOnly startDate, DateOnly endDate)
        {
            ObservableCollection<Order> order = new ObservableCollection<Order>();
            foreach (var item in context.Orders.Include(x => x.Product)
                .Where(o => o.RestaurantId == restaurantId && DateOnly.FromDateTime(o.Data) >= startDate && DateOnly.FromDateTime(o.Data) <= endDate)
                .ToList())
            {
                order.Add(item);
            }
            return order;
        }

        public static ObservableCollection<Order> GetAllUnsolvedOrders(long restaurantId, AppDbContext context)
        {
            ObservableCollection<Order> order = new ObservableCollection<Order>();

            foreach (var item in context.Orders.Include(x => x.Product).Where(o => o.RestaurantId == restaurantId && !o.Solved).ToList())
            {
                order.Add(item);

            }
            return order;
        }
        public static void AddOrder(Order order, AppDbContext context)
        {
            context.Orders.Add(order);
            context.SaveChangesAsync();
        }
        public static void SolveOrder(long orderId, AppDbContext context)
        {
            var order = context.Orders.Find(orderId);
            if (order != null)
            {
                order.Solved = true;
                context.SaveChangesAsync();
            }
        }


        public static List<string> QUANTITY_TYPES = new List<string> { "Stk.", "Tasse", "Kg", "g", "Bd.", "Topf." };
        public static double GetOrderTotalWith10Percent(long restaurantId)
        {
            double total = 0;
            ObservableCollection<Order> orders = GetAllUnsolvedOrders(restaurantId, context);

            foreach (Order order in orders)
            {
                total += order.ProductPrice * order.Amount;
            }
            return total + total * 0.1;
        }

    
      public static double GetOrderTotalUnsolved(long restaurantId)
        {
            double total = 0;
            ObservableCollection<Order> orders = GetAllUnsolvedOrders(restaurantId, context);

            foreach (Order order in orders)
            {
                total += order.ProductPrice * order.Amount;
            }
            return total;
        }
        public static double GetOrderTotal(long restaurantId, DateOnly startDate, DateOnly endDate)
        {
            double total = 0;
            ObservableCollection<Order> orders = GetOrders(restaurantId,  startDate, endDate);

            foreach (Order order in orders)
            {
                total += order.ProductPrice * order.Amount;
            }
            return total;
        }

        public static double GetOrderTotal(long restaurantId)
        {
            double total = 0;
            ObservableCollection<Order> orders = GetAllUnsolvedOrders(restaurantId, context);
            foreach (Order order in orders)
            {
                total += order.ProductPrice * order.Amount;
            }
            return total;
        }



    }
}