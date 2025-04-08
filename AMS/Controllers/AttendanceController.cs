using AMS.Interfaces;
using AMS.Models;
using AMS.Repository;
using CRM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AMS.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public AttendanceController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Dashboard");
            }

            return View();
        }

        public async Task<IActionResult> GetEmployees()
        {
            Console.WriteLine("GetEmployees Controller called");

            var employee = await _adminRepository.GetAllAsync();

            var result = employee.Select(e => new {
                id = e.EmployeeId,
                name = e.FirstName + " " + e.LastName
            });

            return Json(result);
        }

        public async Task<IActionResult> GetEmployeeAttendance(int employee, int month, int year)
        {
            Console.WriteLine("GetEmployeeAttendance Controller called");

            var employeeDetails = await _adminRepository.GetAllAsync();

            var employeeResult = employeeDetails.Select(e => new {
                id = e.EmployeeId,
                name = e.FirstName + " " + e.LastName
            }).ToList();

            var result = await _adminRepository.GetAttendanceByMonthYearAsync(employee, month, year);

            // Join attendance with employee name
            var enrichedResult = result.Select(att => {
                var emp = employeeResult.FirstOrDefault(e => e.id == att.EmployeeID);
                return new
                {
                    employeeID = att.EmployeeID,
                    employeeName = emp != null ? emp.name : "Unknown",
                    attendanceDate = att.AttendanceDate,
                    status = att.Status
                };
            });

            return new JsonResult(enrichedResult);
        }

    }
}


//public async Task<IActionResult> GetEmployeeAttendance(int employee, int month, int year)
//{
//    var employeeDetails = await _adminRepository.GetAllAsync();
//    var attendanceRecords = await _adminRepository.GetAttendanceByMonthYearAsync(employee, month, year);

//    var result = from record in attendanceRecords
//                 join emp in employeeDetails on record.EmployeeID equals emp.EmployeeId
//                 select new
//                 {
//                     record.EmployeeID,
//                     EmployeeName = emp.FirstName + " " + emp.LastName,
//                     record.AttendanceDate,
//                     record.Status
//                 };

//    return new JsonResult(result);
//}
