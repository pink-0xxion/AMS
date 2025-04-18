using AMS.Models;

namespace CRM.Data
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

    }
}
