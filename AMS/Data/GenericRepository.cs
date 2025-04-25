using Dapper;
using System.Data;
using System.Text;
using System.Reflection;
using AMS.Models;
using AMS.Models.ViewModel;

namespace AMS.Data

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

        // Get Attendance By Id or Date // unnecessary, you have used GetByIdAsync instead
        public async Task<IEnumerable<T>> GetAttendanceByIdAsync(string idColumn, object value)
        {
            var query = $"SELECT * FROM {_tableName} WHERE {idColumn} = @Value";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(query, new { Value = value });
        }




        public async Task<Attendance?> GetAttendanceByEmployeeDateAsync(int employeeId, DateTime date)
        {
            using var connection = _context.CreateConnection();
            await connection.OpenAsync(); // Ensure the connection is open

            //var query = "SELECT * FROM Attendance WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate";
            var query = "SELECT * FROM Attendance WHERE EmployeeID = @EmployeeID AND CONVERT(DATE, AttendanceDate) = @AttendanceDate";


            Console.WriteLine($"Executing Query: {query}");
            Console.WriteLine($"Params: EmployeeID = {employeeId}, AttendanceDate = {date:yyyy-MM-dd}");

            //return await connection.QuerySingleOrDefaultAsync<Attendance>(query, new { EmployeeID = employeeId, AttendanceDate = date.Date });
            return await connection.QueryFirstOrDefaultAsync<Attendance>(query, new { EmployeeID = employeeId, AttendanceDate = date.Date });

        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            // Console.WriteLine("UpdateAttendanceAsync called");


            using var connection = _context.CreateConnection();
            await connection.OpenAsync(); // 👈 Ensure the connection is open

       

                var query = @"
            UPDATE Attendance 
            SET 
                CheckOutTime = @CheckOutTime,
                CheckOutLat = @CheckOutLat,
                CheckOutLong = @CheckOutLong,
                CheckoutIP=@CheckoutIP,
                FollowUpShift=@followUpShift

            WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate";

            //await connection.ExecuteAsync(query, new { attendance.CheckOutTime, attendance.EmployeeId, attendance.AttendanceDate });

            await connection.ExecuteAsync(query, new
            {
                attendance.CheckOutTime,
                attendance.CheckOutLat,
                attendance.CheckOutLong,
                attendance.EmployeeId,
                attendance.AttendanceDate,
                attendance.CheckoutIP,
                attendance.FollowUpShift

            });
        }



        public async Task LogCheckOutAsync(
                      int attendanceId,
                      TimeSpan checkInTime,
                      TimeSpan checkOutTime,
                      double? checkInLat,
                      double? checkInLong,
                      double? checkOutLat,
                      double? checkOutLong)
                        {
                            using var connection = _context.CreateConnection();
                            await connection.OpenAsync();

                            var query = @"
                        INSERT INTO AttendanceLogs 
                        (AttendanceID, CheckInTime, CheckOutTime, CheckInLat, CheckInLong, CheckOutLat, CheckOutLong, LogDateTime)
                        VALUES 
                        (@AttendanceID, @CheckInTime, @CheckOutTime, @CheckInLat, @CheckInLong, @CheckOutLat, @CheckOutLong, GETDATE())";

                            await connection.ExecuteAsync(query, new
                            {
                                AttendanceID = attendanceId,
                                CheckInTime = checkInTime,
                                CheckOutTime = checkOutTime,
                                CheckInLat = checkInLat,
                                CheckInLong = checkInLong,
                                CheckOutLat = checkOutLat,
                                CheckOutLong = checkOutLong
                            });
                        }




        //DATA FETCH FROM USER



        public async Task<EmployeeAttendanceDto?> GetEmployeeAttendanceByDateAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();

            string query = @"
            SELECT 
                e.FirstName, 
                e.LastName, 
                e.Department, 
                e.Designation,
                COALESCE(CONVERT(VARCHAR, a.CheckInTime, 108), 'Not Available') AS CheckInTime, 
                COALESCE(CONVERT(VARCHAR, a.CheckOutTime, 108), 'Not Available') AS CheckOutTime, 
                COALESCE(a.Status, 'Not Available') AS Status, 
                a.CheckInLat,
                a.CheckInLong,
                a.CheckOutLat,
                a.CheckOutLong,
                COALESCE(a.FollowUpShift, 'No') AS FollowUpShift
            FROM Employees e
            LEFT JOIN Attendance a 
                ON e.EmployeeId = a.EmployeeId 
                AND CAST(a.AttendanceDate AS DATE) = CAST(GETDATE() AS DATE)
            WHERE e.EmployeeId = @EmployeeId;";

            return await connection.QueryFirstOrDefaultAsync<EmployeeAttendanceDto>(query, new { EmployeeId = employeeId });
        }







     


        public async Task<bool> CheckInAsync(int employeeId, string ip, double? checkInLat, double? checkInLong, string followUpShift)
        {
            var sql = @"
                    IF EXISTS (SELECT 1 FROM Attendance WHERE EmployeeId = @EmployeeId AND AttendanceDate = CAST(GETDATE() AS DATE))
                    BEGIN
                        UPDATE Attendance 
                        SET CheckInTime = @CheckInTime, 
                            Status = 'Present',  
                            CheckinIP = @CheckinIP,
                            CheckInLat = @CheckInLat,
                            CheckInLong = @CheckInLong,
                            FollowUpShift = @FollowUpShift
                        WHERE EmployeeId = @EmployeeId AND AttendanceDate = CAST(GETDATE() AS DATE);
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Attendance 
                            (EmployeeId, AttendanceDate, CheckInTime, Status, CheckinIP, CheckInLat, CheckInLong, FollowUpShift)
                        VALUES 
                            (@EmployeeId, GETDATE(), @CheckInTime, 'Present', @CheckinIP, @CheckInLat, @CheckInLong, @FollowUpShift);
                    END";

                        var parameters = new
                        {
                            EmployeeId = employeeId,
                            CheckInTime = DateTime.Now.TimeOfDay,
                            CheckinIP = ip,
                            CheckInLat = checkInLat,
                            CheckInLong = checkInLong,
                            FollowUpShift = followUpShift ?? "No" // Default to "No" if not provided
                        };

                        using var connection = _context.CreateConnection();
                        var result = await connection.ExecuteAsync(sql, parameters);
                        return result > 0;
        }











        // Get Attendance By Id
        //public async Task<IEnumerable<T>> GetAttendanceByIdAsync(string idColumn, int id)
        //{
        //    var query = $"SELECT * FROM {_tableName} WHERE {idColumn} = @Id";
        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryAsync<T>(query, new { Id = id });
        //}




        public async Task<IEnumerable<AttendanceLogDto>> GetAttendanceLogsAsync(int employeeId, int year, int month, int day)
        {
            using var connection = _context.CreateConnection();

            DateTime targetDate;
            try
            {
                targetDate = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Enumerable.Empty<AttendanceLogDto>();
            }


            string query = @"
SELECT 
    COALESCE(CONVERT(VARCHAR, al.LogDateTime, 120), 'Not Available') AS LogDateTime,
    COALESCE(CONVERT(VARCHAR, al.CheckInTime, 108), 'Not Available') AS CheckInTime, 
    COALESCE(CONVERT(VARCHAR, al.CheckOutTime, 108), 'Not Available') AS CheckOutTime
FROM AttendanceLogs al
INNER JOIN Attendance a ON a.AttendanceID = al.AttendanceID
WHERE a.EmployeeID = @EmployeeID AND CAST(al.LogDateTime AS DATE) = @TargetDate
ORDER BY al.LogDateTime DESC;";


            return await connection.QueryAsync<AttendanceLogDto>(query, new
            {
                EmployeeID = employeeId,
                TargetDate = targetDate.Date
            });
        }


        public async Task<IEnumerable<EmpAttendanceDto>> GetAttendanceByMonthYearAsyncById(int employeeId, int month, int year)
        {
            Console.WriteLine("Generic Repository");

            using var connection = _context.CreateConnection();

            string query = @"
        SELECT 
            e.EmployeeId,
            (e.FirstName + ' ' + e.LastName) AS EmployeeName,
            a.AttendanceDate,
            a.Status
        FROM Attendance a
        INNER JOIN Employees e ON a.EmployeeId = e.EmployeeId
        WHERE MONTH(a.AttendanceDate) = @Month
          AND YEAR(a.AttendanceDate) = @Year
          AND a.EmployeeId = @EmployeeId
        ORDER BY a.AttendanceDate";

            var result = await connection.QueryAsync<EmpAttendanceDto>(query, new
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year
            });

            return result;
        }




    }
}
