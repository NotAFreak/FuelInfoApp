using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MobileNumber { get; set; }

    public int? Postcode { get; set; }

    public int? Latitude { get; set; }

    public int? Longitude { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual UserPreference? UserPreference { get; set; }
}
