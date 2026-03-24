using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
