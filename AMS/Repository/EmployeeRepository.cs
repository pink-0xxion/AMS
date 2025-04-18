using AMS.Interfaces;
using AMS.Models;
using AMS.Data;

namespace AMS.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        public IGenericRepository<Attendance> _employeeAttendance { get; }
        

        public EmployeeRepository(IGenericRepository<Attendance> employeeAttendance) 
        
        {

            _employeeAttendance = employeeAttendance;

        }








        
        public Task<Attendance?> GetAttendanceByEmployeeDateAsync(int employeeId, DateTime date)
        {
            return _employeeAttendance.GetAttendanceByEmployeeDateAsync(employeeId, date);
        }



        public Task UpdateAttendanceAsync(Attendance attendance)
        {
            return _employeeAttendance.UpdateAttendanceAsync(attendance);
        }




        //attendCon
        public Task<EmployeeAttendanceDto?> GetEmployeeAttendanceByDateAsync(int employeeId)
        {
           return _employeeAttendance.GetEmployeeAttendanceByDateAsync(employeeId);
        }



        //public async Task<int> AddAttendanceAsync(Attendance attendance)
        //{
        //    return await _employeeAttendance.AddAttendanceAsync(attendance);
        //}

        public Task<bool> CheckInAsync(int employeeId, string remarks)
        {
         return _employeeAttendance.CheckInAsync(employeeId, remarks);
        }




        public Task LogCheckOutAsync(int attendanceId, TimeSpan checkInTime, TimeSpan checkOutTime)
        {
            return _employeeAttendance.LogCheckOutAsync(attendanceId, checkInTime, checkOutTime);
        }

        public Task<IEnumerable<AttendanceLogDto>> GetAttendanceLogsAsync(int employeeId, int year, int month, int day)
        {
            return _employeeAttendance.GetAttendanceLogsAsync(employeeId, year, month, day);
        }

        public Task<IEnumerable<EmpAttendanceDto>> GetAttendanceByMonthYearAsyncById(int employeeId, int month, int year)
        {
            return _employeeAttendance.GetAttendanceByMonthYearAsyncById(employeeId, month, year);
        }
    }
}
