namespace AMS.Models
{
    public class AttendanceLogDto
    {
        public DateTime LogDateTime { get; set; }


        // These should be TimeSpan if the SQL column is a `time` type
        public string CheckInTime { get; set; } = "Not Available";
        public string CheckOutTime { get; set; } = "Not Available";
    }
}