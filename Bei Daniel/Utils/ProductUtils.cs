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
    }

}