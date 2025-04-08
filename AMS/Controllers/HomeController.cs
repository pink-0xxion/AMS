using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AMS.Models;

namespace AMS.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("UserSession") != null)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        if (HttpContext.Session.GetString("EmployeeSession") != null)
        {
            return RedirectToAction("Index", "Employee", new { area = "Employee" });
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
