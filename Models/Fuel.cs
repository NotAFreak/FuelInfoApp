using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

public partial class Fuel
{
    public int FuelId { get; set; }

    public string FuelName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    [JsonIgnore]
    public virtual ICollection<StationFuelChange> StationFuelChanges { get; set; } = new List<StationFuelChange>();
}
