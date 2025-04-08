using System;
using System.Collections.Generic;

namespace AMS.Models;

public partial class LeaveRequest
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    //public virtual Employees Employee { get; set; } = null!;
}
