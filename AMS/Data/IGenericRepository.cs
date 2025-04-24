﻿using AMS.Models;
using AMS.Models.ViewModel;

namespace AMS.Data

{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); // Get all data
        Task<T> GetByIdAsync(string idColumn, int id); // Get by ID
        Task<int> InsertAsync(T entity); // Insert new
        Task<int> UpdateAsync(string idColumn, T entity); // Update by ID
        Task<int> DeleteAsync(string idColumn, int id); // Delete by ID

        // ✅ method to match login credentials (username and password)
        Task<T> GetByCredentialsAsync(string usernameColumn, string passwordColumn, string username, string password);

        // ✅ New method to match login credentials (username and password)
        Task<T> GetByUserCredentialsAsync(string usernameColumn, string passwordColumn, string roleColumn, string username, string password, string role);

        // ✅ New method: Get attendance records optionally filtered by employee
        Task<IEnumerable<dynamic>> GetAttendanceByMonthYearAsync(int employee, int month, int year);

        // Get Attendance By Id or Date
        Task<IEnumerable<T>> GetAttendanceByIdAsync(string idColumn, object value);



        Task<bool> CheckInAsync(int employeeId, string ip, double? checkInLat, double? checkInLong);


        //not in use 
        //Task<int> AddAttendanceAsync(Attendance attendance);
        // till here

        Task<Attendance?> GetAttendanceByEmployeeDateAsync(int employeeId, DateTime date);

        Task UpdateAttendanceAsync(Attendance attendance);


        //DATA FETCH FROM USER


        Task<EmployeeAttendanceDto?> GetEmployeeAttendanceByDateAsync(int employeeId);



        Task LogCheckOutAsync(int attendanceId, TimeSpan checkInTime, TimeSpan checkOutTime);





        // Get Attendance By Id
        //Task<IEnumerable<T>> GetAttendanceByIdAsync(string idColumn, int id);


        Task<IEnumerable<AttendanceLogDto>> GetAttendanceLogsAsync(int employeeId, int year, int month, int day);

        Task<IEnumerable<EmpAttendanceDto>> GetAttendanceByMonthYearAsyncById(int employeeId, int month, int year);



    }
}
