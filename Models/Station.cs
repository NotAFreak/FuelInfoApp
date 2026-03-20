using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Station
{
    public int StationId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string Name { get; set; } = null!;

    public int BrandId { get; set; }

    public string? Address { get; set; }

    public string? Suburb { get; set; }

    public string State { get; set; } = null!;

    public int? Postcode { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<StationFuelChange> StationFuelChanges { get; set; } = new List<StationFuelChange>();
}
