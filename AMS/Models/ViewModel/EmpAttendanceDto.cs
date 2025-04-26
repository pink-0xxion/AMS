namespace AMS.Models.ViewModel
{
    public class EmpAttendanceDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
