using Dapper;
using System.Data;
using System.Text;
using System.Reflection;

namespace CRM.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DapperContext _context;
        private readonly string _tableName;

        public GenericRepository(DapperContext context)
        {
            _context = context;
            _tableName = typeof(T).Name; // Assumes table name = class name
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = $"SELECT * FROM {_tableName}";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(query);
        }

        public async Task<T> GetByIdAsync(string idColumn, int id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE {idColumn} = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        public async Task<int> InsertAsync(T entity)
        {
            //var props = typeof(T).GetProperties()
            //    .Where(p =>
            //    {
            //        var attr = p.GetCustomAttributes(true);
            //        return !attr.Any(a =>
            //            a is System.ComponentModel.DataAnnotations.KeyAttribute ||
            //            (a is System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute dg && dg.DatabaseGeneratedOption == System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)
            //        );
            //    })
            //    .ToList();
            var props = typeof(T).GetProperties()
                .Where(p =>
                    !(p.Name.ToLower().EndsWith("id") && p.PropertyType == typeof(int)) // Exclude identity column
                )
                .ToList();
            var columnNames = string.Join(", ", props.Select(p => p.Name));
            var paramNames = string.Join(", ", props.Select(p => "@" + p.Name));
            var query = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({paramNames})";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> UpdateAsync(string idColumn, T entity)
        {
            var props = typeof(T).GetProperties().Where(p => p.Name.ToLower() != idColumn.ToLower()).ToList();
            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
            var query = $"UPDATE {_tableName} SET {setClause} WHERE {idColumn} = @{idColumn}";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, entity);
        }

        public async Task<int> DeleteAsync(string idColumn, int id)
        {
            var query = $"DELETE FROM {_tableName} WHERE {idColumn} = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id });
        }

        // ✅ method: Login query using username and password columns
        public async Task<T> GetByCredentialsAsync(string usernameColumn, string passwordColumn, string username, string password)
        {
            var query = $"SELECT * FROM {_tableName} WHERE {usernameColumn} = @Username AND {passwordColumn} = @Password";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(query, new { Username = username, Password = password });
        }

        // ✅ New method: Login query using username and password columns
        public async Task<T> GetByUserCredentialsAsync(string usernameColumn, string passwordColumn, string roleColumn, string username, string password, string role)
        {
            Console.WriteLine("GetByUserCredentialsAsync Called");

            var query = $"SELECT * FROM [{_tableName}] WHERE {usernameColumn} = @Username AND {passwordColumn} = @Password AND {roleColumn} = @Role";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(query, new { Username = username, Password = password, Role = role });
        }

        // ✅ New method: Get attendance records optionally filtered by employee
        public async Task<IEnumerable<dynamic>> GetAttendanceByMonthYearAsync(int employee, int month, int year)
        {
            using var connection = _context.CreateConnection();

            string query = @"SELECT 
                                AttendanceDate,
                                CheckInTime,
                                CheckOutTime,
                                Status,
                                Remarks,
                                EmployeeID
                            FROM Attendance
                            WHERE MONTH(AttendanceDate) = @Month
                              AND YEAR(AttendanceDate) = @Year";

            if (employee != 0)
            {
                query += " AND EmployeeID = @EmployeeID";
            }

            return await connection.QueryAsync(query, new
            {
                EmployeeID = employee,
                Month = month,
                Year = year
            });
        }
    }
}
