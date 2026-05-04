using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Station
{
    public int StationId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Name { get; set; } = null!;

    public int BrandId { get; set; }

    public string? Address { get; set; }

    public string? Suburb { get; set; }

    public string State { get; set; } = null!;

    public int? Postcode { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<StationFuelState> StationFuelStates { get; set; } = new List<StationFuelState>();
}
