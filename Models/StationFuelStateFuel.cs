using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptOut)]
public partial class StationFuelStateFuel
{
    public int StateId { get; set; }

    public int FuelId { get; set; }

    public int Price { get; set; }

    public int IsAvailable { get; set; }

    public virtual Fuel Fuel { get; set; } = null!;

    [JsonIgnore]
    public virtual StationFuelState State { get; set; } = null!;
}
