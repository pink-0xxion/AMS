﻿using System;
using System.Collections.Generic;

namespace AMS.Models;

public partial class Attendance
{
    public int AttendanceID { get; set; }

    public int EmployeeId { get; set; }

    //public DateOnly AttendanceDate { get; set; }


    public DateTime AttendanceDate { get; set; }

    //public TimeOnly CheckInTime { get; set; }

    public TimeSpan CheckInTime { get; set; }

    //public TimeOnly? CheckOutTime { get; set; }


    //public TimeOnly? CheckOutTime { get; set; }

    public TimeSpan? CheckOutTime { get; set; }

    public string Status { get; set; } = null!;


    public string? Remarks { get; set; }

    //public virtual Employees Employee { get; set; } = null!;

    //public string? RemarksOut { get; set; }

    public string? FollowUpShift { get; set; }

    public string? CheckinIP { get; set; }
    public string? CheckoutIP { get; set; }

    public double? CheckInLat { get; set; }
    public double? CheckInLong { get; set; }
    public double? CheckOutLat { get; set; }
    public double? CheckOutLong { get; set; }

}
