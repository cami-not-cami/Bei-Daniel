using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bei_Daniel.Models;

public partial class Order
{
    public long Id { get; set; }

    public long RestaurantId { get; set; }

    public long ProductId { get; set; }

    public long Amount { get; set; }

    public double ProductPrice { get; set; }

    public DateTime Data { get; set; }

    public bool Solved { get; set; }
    [NotMapped] public double InLineTotal { get; set; }
    //[NotMapped] public string CompletedAmount { get; set; } = string.Empty;

    public virtual Product Product { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
