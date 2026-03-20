using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Car
{
    public int CarId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Registration { get; set; }

    public int FuelId { get; set; }

    public decimal FuelConsumption { get; set; }

    public virtual Fuel Fuel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
