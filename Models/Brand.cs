using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public int BrandName { get; set; }

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
