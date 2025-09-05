using System;
using System.Collections.Generic;

namespace Bei_Daniel.Models;

public partial class Restaurant
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
