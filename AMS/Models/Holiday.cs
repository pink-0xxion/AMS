using System;
using System.Collections.Generic;

namespace AMS.Models;

public partial class Holiday
{
    public int HolidayId { get; set; }

    public string HolidayName { get; set; } = null!;

    public DateOnly HolidayDate { get; set; }

    public string? Description { get; set; }
}
