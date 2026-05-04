using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptOut)]
public partial class StationFuelState
{
    public int StateId { get; set; }

    public int StationId { get; set; }

    public byte[] ChangeTime { get; set; } = null!;
    
    [JsonIgnore]
    public virtual Station Station { get; set; } = null!;

    public virtual ICollection<StationFuelStateFuel> StationFuelStateFuels { get; set; } = new List<StationFuelStateFuel>();
}
