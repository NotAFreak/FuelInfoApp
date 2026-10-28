using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptOut)]
public partial class StationFuelChange
{
    public int ChangeId { get; set; }

    public int StationId { get; set; }

    public int FuelId { get; set; }

    public byte[] ChangeTime { get; set; } = null!;

    public int Price { get; set; }

    public int IsAvailable { get; set; }
    
    public virtual Fuel Fuel { get; set; } = null!;
    [JsonIgnore]
    public virtual Station Station { get; set; } = null!;
}
