using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FuelInfoApp.Models;

[JsonObject(MemberSerialization.OptOut)]
public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? BrandImageUrl { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
