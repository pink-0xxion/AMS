using System.Runtime.InteropServices;
using System.Text.Json;
using AMS.Interfaces;
using AMS.Models;
using AMS.Models.ViewModel;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using AMS.Services;
using AMS.Helpers;
using QuestPDF.Fluent;

namespace AMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly PdfService _pdfService;
        private readonly IViewRenderService _viewRenderService;


        public DashboardController(IAdminRepository adminRepository, PdfService pdfservice, IViewRenderService viewRenderService)
        {
            _adminRepository = adminRepository;
            _pdfService = pdfservice;
            _viewRenderService = viewRenderService;
        }


        public async Task<IActionResult> Index()
        {
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var employee = await _adminRepository.GetAllAsync();
            return View(employee);
        }

        //------------------------------------OLD LOGIN-----------------------------------------------//
        //public IActionResult Login()
        //{
        //    Console.WriteLine("Login called");
        //    if (HttpContext.Session.GetString("UserSession") != null)
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(Models.Admin admin)
        //{
        //    Console.WriteLine("joker");
        //    Console.WriteLine(admin.Email);
        //    Console.WriteLine(admin.Password);
        //    // Replace these with your actual column names from the database
        //    string usernameColumn = "Email";
        //    string passwordColumn = "Password";

        //    var myAdmin = await _adminRepository.GetByCredentialsAsync(usernameColumn, passwordColumn, admin.Email, admin.Password);

        //    Console.WriteLine("show it: " + JsonSerializer.Serialize(myAdmin));


        //    if (myAdmin != null)
        //    {
        //        HttpContext.Session.SetString("UserSession", myAdmin.Email);
        //        //HttpContext.Session.SetString("UserImage", myAdmin.Image ?? "default-profile.jpg");
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Login Failed...";
        //    }
        //    return View();
        //}
        //------------------------------------OLD LOGIN-----------------------------------------------//



        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            Console.WriteLine("joker");
            Console.WriteLine(user.Username);
            Console.WriteLine(user.PasswordHash);
            // Replace these with your actual column names from the database
            string usernameColumn = "Username";
            string passwordColumn = "PasswordHash";
            string roleColumn = "Role";

            var myAdmin = await _adminRepository.GetByUserCredentialsAsync(usernameColumn, passwordColumn, roleColumn, user.Username, user.PasswordHash, user.Role);

            Console.WriteLine("show it: " + JsonSerializer.Serialize(myAdmin));

            if (myAdmin != null)
            {
                if (myAdmin.Role == "Admin" || myAdmin.Role == "admin")
                {
                    HttpContext.Session.SetString("UserSession", myAdmin.Username);
                    //HttpContext.Session.SetString("UserImage", myAdmin.Image ?? "default-profile.jpg");
                    return RedirectToAction("Index", "Attendance", new { area = ""});
                }
                else if (myAdmin.Role == "Employee" || myAdmin.Role == "employee")
                {
                    HttpContext.Session.SetInt32("EmployeeId", myAdmin.EmployeeId.Value);
                    //HttpContext.Session.SetString("UserImage", myAdmin.Image ?? "default-profile.jpg");
                    return RedirectToAction("Index", "Employee", new { area = "Employee" });
                }
                else
                {
                    TempData["Error"] = "Login Failed...";
                }
            }
            else
            {
                TempData["Error"] = "Login Failed...";
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult Create()
        {
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("FirstName, LastName, Email, PhoneNumber, Department, Designation, JoiningDate, Status")] Employees employee)
        {
            Console.WriteLine("Create Post called");
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                // Set only the date part (time becomes 00:00:00)
                //employee.JoiningDate = employee.JoiningDate.Date;

                @employee.JoiningDate.ToString("dd/MM/yyyy");

                Console.WriteLine("Date: " + employee.JoiningDate);

                var result = await _adminRepository.InsertAsync(employee);

                if (result != null)
                {
                    TempData["Notification"] = "Employee Added Successfully";
                    TempData["NotificationType"] = "success";
                }

                return RedirectToAction("Index");
            }
            // Debugging Required fields
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Create: ModelState is NOT valid!");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(employee);
            }
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Console.WriteLine("Edit: Get");

            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            string idColumn = "EmployeeId";
            var employee = await _adminRepository.GetByIdAsync(idColumn, id);

            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return View(employee);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("EmployeeId, FirstName, LastName, Email, PhoneNumber, Department, Designation, JoiningDate, Status")] Employees employee)
        {
            var email = HttpContext.Session.GetString("UserSession"); // Use logged-in user's email

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" }); // Redirect if not logged in
            }

            if (ModelState.IsValid)
            {

                string idColumn = "EmployeeId";
                var existingEmployee = await _adminRepository.GetByIdAsync(idColumn, employee.EmployeeId);

                if (existingEmployee == null)
                {
                    return NotFound();
                }

                //var query1 = "UPDATE Products SET Name = @Name, Price = @Price, Description = @Description, Stock = @Stock WHERE Id = @Id";
                //await _db.ExecuteAsync(query1, product);

                var result = await _adminRepository.UpdateAsync(idColumn, employee);

                if (result != null)
                {
                    TempData["Notification"] = "Employ Updated Successfully";
                    TempData["NotificationType"] = "success";
                }

                return RedirectToAction("Index");
            }

            // Required Field Debugging
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Edit: ModelState is NOT valid!");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
                return View(employee);
            }

            return View(employee);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            string idColumn = "EmployeeId";
            var existingEmployee = await _adminRepository.GetByIdAsync(idColumn, id);

            Console.WriteLine("GET: ID Value: " + id);
            Console.WriteLine("GET: existingEmployee: " + existingEmployee);

            if (existingEmployee == null)
            {
                return NotFound();
            }
            return View(existingEmployee);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int EmployeeId)
        {
            Console.WriteLine("ConfirmDelete Called");

            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            string idColumn = "EmployeeId";
            var existingEmployee = await _adminRepository.GetByIdAsync(idColumn, EmployeeId);

            Console.WriteLine("ID Value: " + EmployeeId);
            Console.WriteLine("existingEmployee: " + existingEmployee);

            if (existingEmployee != null)
            {
                Console.WriteLine("existingEmployee Called");

                var result = await _adminRepository.DeleteAsync(idColumn, EmployeeId);

                if (result != null)
                {
                    TempData["Notification"] = "Employee Deleted Successfully";
                    TempData["NotificationType"] = "success";
                }

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var email = HttpContext.Session.GetString("UserSession");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            string idColumn = "EmployeeId";

            var employee = await _adminRepository.GetByIdAsync(idColumn, id);
            var attendence = await _adminRepository.GetAttendanceByIdAsync(idColumn, id);

            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = employee,
                AttendanceRecord = attendence.ToList()
            };

            return View(viewModel);
        }

        //  Download Pdf
        public async Task<IActionResult> DownloadPdf(int id)
        {
            /*  //string pageUrl = Url.Action("EmployeeDetails", "Dashboard", new { id = id }, Request.Scheme, Request.Host.Value);

              //string pageUrl = Url.Action(
              //    action: "EmployeeDetails",
              //    controller: "Dashboard",
              //    values: new { area = "Admin", id = id },
              //    protocol: Request.Scheme,
              //    host: Request.Host.Value
              //);

              //string pageUrl = "https://localhost:7067/Admin/Dashboard/EmployeeDetails/" + id; // Authentication cookie required
              string pageUrl = "https://mdbootstrap.com/docs/standard/extended/bootstrap4-table-scroll/";

              var pdfBytes = _pdfService.GeneratePdf(pageUrl);
              return File(pdfBytes, "application/pdf", "EmployeeDetails.pdf"); // To download the PDF File
            */

            string idColumn = "EmployeeId";

            var employee = await _adminRepository.GetByIdAsync(idColumn, id);
            var attendance = await _adminRepository.GetAttendanceByIdAsync(idColumn, id);

            var model = new EmployeeDetailsViewModel
            {
                Employee = employee,
                AttendanceRecord = attendance.ToList()
            };

            var htmlContent = await _viewRenderService.RenderViewAsync(this.ControllerContext, "EmployeeDetails", model);

            var pdfBytes = _pdfService.GeneratePdfFromHtml(htmlContent);

            return File(pdfBytes, "application/pdf", "EmployeeDetails.pdf");
        }

        public async Task<IActionResult> DownloadQuestPdf(int id)
        {
            string idColumn = "EmployeeId";

            var employee = await _adminRepository.GetByIdAsync(idColumn, id);
            var attendance = await _adminRepository.GetAttendanceByIdAsync(idColumn, id);

            var model = new EmployeeDetailsViewModel
            {
                Employee = employee,
                AttendanceRecord = attendance.ToList()
            };

            var document = new EmployeeDetailsDocument(model);
            var pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", "EmployeeDetails_QuestPDF.pdf");
        }

        public IActionResult Logout()
        {

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                //HttpContext.Session.Remove("UserSession");
                //HttpContext.Session.Remove("UserImage");
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }



        public IActionResult Test()
        {
            Console.WriteLine("Test Called");
            return View();
        }

    }
}
