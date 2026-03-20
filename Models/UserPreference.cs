using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class UserPreference
{
    public int UserId { get; set; }

    public int DarkMode { get; set; }

    public decimal PriceWeight { get; set; }

    public int DistanceWeight { get; set; }

    public virtual User User { get; set; } = null!;
}
