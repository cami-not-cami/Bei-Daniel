using Bei_Daniel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bei_Daniel.Utils
{
    internal class ProductUtils
    {
        public static string TransformScaleNumberToName(int scaleNumber, AppDbContext context)
        {
            return context.Products.Where(p => p.ProductScaleNumber == scaleNumber)
                .Select(p => p.Name)
                .FirstOrDefault();
        }
        public static string GetProductNameFromInput(string input, AppDbContext context)
        {
            if (int.TryParse(input, out int scaleNumber))
            {
                return TransformScaleNumberToName(scaleNumber, context);
            }
            else
            {
                return input;
            }

        }
        public static long GetProductIdFromName(string name, AppDbContext context)
        {
            return context.Products.Where(p => p.Name == name)
                  .Select(p => p.Id)
                  .FirstOrDefault();
        }

        public static ObservableCollection<Order> BundleIdenticalProducts(ObservableCollection<Order> orders)
        {
            ObservableCollection<Order> bundledOrders = new ObservableCollection<Order>();

            // Group orders by ProductId, ProductPrice, and ProductQuantityType
            var groupedOrders = orders
                .GroupBy(o => new { o.ProductId, o.ProductPrice, o.ProductQuantityType })
                .Select(g => new Order
                {
                    Id = g.First().Id,
                    RestaurantId = g.First().RestaurantId,
                    ProductId = g.Key.ProductId,
                    Amount = g.Sum(o => o.Amount),
                    ProductPrice = g.Key.ProductPrice,
                    Data = g.First().Data,
                    Solved = g.First().Solved,
                    ProductQuantityType = g.Key.ProductQuantityType,
                    Product = g.First().Product,
                    Restaurant = g.First().Restaurant,
                    InLineTotal = g.Sum(o => o.Amount) * g.Key.ProductPrice
                });

            foreach (var order in groupedOrders)
            {
                bundledOrders.Add(order);
            }

            return bundledOrders;
        }
    }

}