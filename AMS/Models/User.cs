using System;
using System.Collections.Generic;

namespace AMS.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? EmployeeId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    //public virtual Employees? Employee { get; set; }
}
