using System;
using System.Collections.Generic;

namespace FuelInfoApp.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
