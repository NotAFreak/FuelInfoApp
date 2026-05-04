using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptOut)]
public partial class Fuel
{
    public int FuelId { get; set; }

    public string FuelName { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    
    [JsonIgnore]
    public virtual ICollection<StationFuelStateFuel> StationFuelStateFuels { get; set; } = new List<StationFuelStateFuel>();
}
