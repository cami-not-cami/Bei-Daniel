using System;
using System.Collections.Generic;

namespace Bei_Daniel.Models;

public partial class Product
{
    public long Id { get; set; }

    public int ProductScaleNumber { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
