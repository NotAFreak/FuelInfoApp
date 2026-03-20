using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class StationFuelChange
{
    public int StationId { get; set; }

    public int FuelId { get; set; }

    public byte[] ChangeTime { get; set; } = null!;

    public int Price { get; set; }

    public int IsAvailable { get; set; }

    public virtual Fuel Fuel { get; set; } = null!;

    public virtual Station Station { get; set; } = null!;
}
