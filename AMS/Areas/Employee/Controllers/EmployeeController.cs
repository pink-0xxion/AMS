using Microsoft.AspNetCore.Mvc;

namespace AMS.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("EmployeeSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new {area = ""});
            }

            return View();
        }
    }
}
