using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Fuel
{
    public int FuelId { get; set; }

    public string FuelName { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<StationFuelChange> StationFuelChanges { get; set; } = new List<StationFuelChange>();
}
