using System;
using System.Collections.Generic;

namespace AMS.Models;

public partial class Employees
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public DateTime JoiningDate { get; set; }

    public string Status { get; set; } = null!;

    //public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    //public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    //public virtual User? User { get; set; }
}
