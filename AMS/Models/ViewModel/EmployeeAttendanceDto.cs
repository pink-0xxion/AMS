namespace AMS.Models.ViewModel
{
    public class EmployeeAttendanceDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = "Not Available";
        public string CheckOutTime { get; set; } = "Not Available";
        public string Status { get; set; } = "Not Available";

        //public string Remarks { get; set; } = "Not Available";
        //public string RemarksOut { get; set; } = "Not Available";


        public double? CheckInLat { get; set; }
        public double? CheckInLong { get; set; }
        public double? CheckOutLat { get; set; }
        public double? CheckOutLong { get; set; }

    }
}
