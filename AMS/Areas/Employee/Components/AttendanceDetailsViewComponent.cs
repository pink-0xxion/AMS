using AMS.Interfaces;
using AMS.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Areas.Employee.Components
{
    public class AttendanceDetailsViewComponent : ViewComponent
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AttendanceDetailsViewComponent(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
            {
                return View("Default", new EmployeeAttendanceDto()); // Show "Not Available" if not logged in
            }

            var attendance = await _employeeRepository.GetEmployeeAttendanceByDateAsync(employeeId.Value);
            return View("Default", attendance ?? new EmployeeAttendanceDto());
        }
    }
}
