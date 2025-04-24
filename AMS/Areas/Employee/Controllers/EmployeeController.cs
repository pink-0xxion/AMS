﻿using AMS.Interfaces;
using AMS.Models;
using AMS.Models.ViewModel;
using AMS.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AMS.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class EmployeeController : Controller
    {

        private readonly IEmployeeRepository _employeeRepository;


        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            ViewBag.EmployeeId = employeeId.Value;


            var today = DateTime.Today;

            var logs = await _employeeRepository.GetAttendanceLogsAsync(
                employeeId.Value,
                today.Year,
                today.Month,
                today.Day
            );

            var viewModel = new EmployeeAttendanceViewModel
            {
                AttendanceLogs = (List<AttendanceLogDto>)logs
            };

            return View(viewModel);

        }




        [HttpPost]
        public async Task<IActionResult> CheckIn(int employeeId, string location, string followUpShift)
        {
            Console.WriteLine("CheckIn Controller called");


            var forwardedIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            var ip = string.IsNullOrEmpty(forwardedIp)
                ? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                : forwardedIp;

            Console.WriteLine($"User IP: {ip}");
            //Console.WriteLine($"User IP: {HttpContext.Connection.RemoteIpAddress}");
            
            // Use it for logging, storing, etc.


            var checkInTime = DateTime.Now.TimeOfDay; // Use TimeSpan

            double? checkInLat = null;
            double? checkInLong = null;

            if (!string.IsNullOrWhiteSpace(location))
            {
                var parts = location.Split(',');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double lat) &&
                    double.TryParse(parts[1], out double lng))
                {
                    checkInLat = lat;
                    checkInLong = lng;
                }
            }

            Console.WriteLine($"1Lat: {checkInLat},1Long: {checkInLong}");

        


            // string remarks = $"Checked in from {location}";
            bool isCheckedIn = await _employeeRepository.CheckInAsync(employeeId,ip, checkInLat, checkInLong, followUpShift);


            if (isCheckedIn)
            {
                return Ok(new { message = "Checked in successfully", checkInTime });
            }

            return BadRequest(new { message = "Failed to check in" });
        }









        private static readonly HashSet<int> _processingCheckOuts = new HashSet<int>(); // Global hash set to track processing employees

        [HttpPost]
        public async Task<IActionResult> CheckOut(int employeeId, string location , string followUpShift)
        {
            if (_processingCheckOuts.Contains(employeeId))
            {
                Console.WriteLine($"🚨 Duplicate CheckOut request blocked for Employee ID {employeeId}");
                return BadRequest(new { message = "Duplicate check-out request detected. Please wait." });
            }

            try
            {
                _processingCheckOuts.Add(employeeId); // Mark employee as processing

                Console.WriteLine("CheckOut Controller called");

                var today = DateTime.Now;
                var checkOutTime = DateTime.Now.TimeOfDay;

                var attendance = await _employeeRepository.GetAttendanceByEmployeeDateAsync(employeeId, today);

                if (attendance == null)
                {
                    return BadRequest(new { message = "No check-in record found for today" });
                }

                Console.WriteLine("After GetAttendanceByEmployeeDateAsync");

                //ip update
                var forwardedIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                var ip = string.IsNullOrEmpty(forwardedIp)
                    ? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    : forwardedIp;

                Console.WriteLine($"User IP: {ip}");



                double? checkOutLat = null;
                double? checkOutLong = null;

                if (!string.IsNullOrWhiteSpace(location))
                {
                    var parts = location.Split(',');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double lat) &&
                        double.TryParse(parts[1], out double lng))
                    {
                        checkOutLat = lat;
                        checkOutLong = lng;
                    }
                }


                var FollowUpShift = followUpShift;

                attendance.CheckOutTime = checkOutTime;
                attendance.CheckOutLat = checkOutLat;
                attendance.CheckOutLong = checkOutLong;
                attendance.CheckoutIP = ip;
                attendance.FollowUpShift = FollowUpShift;

                await _employeeRepository.UpdateAttendanceAsync(attendance);

                await _employeeRepository.LogCheckOutAsync(attendance.AttendanceID, attendance.CheckInTime, checkOutTime);

                return Ok(new { message = "Checked out successfully", checkOutTime });
            }
            finally
            {
                _processingCheckOuts.Remove(employeeId); // Remove from tracking after processing
            }
        }







        [HttpGet]
        public IActionResult LoadAttendanceDetails()
        {
            return ViewComponent("AttendanceDetails"); // This will call AttendanceDetailsViewComponent
        }




        [HttpGet]
        public async Task<IActionResult> AttendanceLog(int year, int month, int day)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
            {
                return Unauthorized();
            }

            var logs = await _employeeRepository.GetAttendanceLogsAsync(employeeId.Value, year, month, day);

            return PartialView("_AttendanceLogRows", logs);
        }









        public async Task<IActionResult> GetEmployeeAttendanceById(int employeeId, int month, int year)
        {
            //Console.WriteLine($"GetEmployeeAttendanceById Controller called" , employeeId, month, year);
            Console.WriteLine($"GetEmployeeAttendanceById Controller called with EmployeeID: {employeeId}, Month: {month}, Year: {year}");


            // Fetch attendance data from the repository
            var attendanceData = await _employeeRepository.GetAttendanceByMonthYearAsyncById(employeeId, month, year);

            Console.WriteLine("attendanceData", attendanceData);
            // If no data is found, return a message
            if (attendanceData == null || !attendanceData.Any())
            {
                return Json(new { message = "No attendance records found for this employee." });
            }

            // Return attendance data as JSON
            return Json(attendanceData);
        }






    }
}
