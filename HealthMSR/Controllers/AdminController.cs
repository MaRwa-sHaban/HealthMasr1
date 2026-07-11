using HealthMSR.BLL.Services;
//using HealthMSR.BLL.ViewModels;
using HealthMSR.DAL.Models;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public IActionResult Dashboard(string section = "overview",
            string specialty = null, string search = null)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var vm = new AdminDashboardViewModel
            {
                ActiveSection = section,
                FilterSpecialty = specialty,
                SearchQuery = search,
                TotalPatients = _adminService.CountPatients(),
                TotalDoctors = _adminService.CountDoctors(),
                TotalPharmacists = _adminService.CountPharmacists(),
                TotalLabTechs = _adminService.CountLabTechs(),
                TotalRadiologists = _adminService.CountRadiologists(),
                Doctors = _adminService.GetAllDoctors(specialty, search),
                Pharmacists = _adminService.GetAllPharmacists(),
                LabTechnicians = _adminService.GetAllLabTechs(),
                Radiologists = _adminService.GetAllRadiologists(),
                Patients = _adminService.GetAllPatients(),
                Organizations = _adminService.GetAllOrganizations()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddDoctor(IFormCollection f)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.AddDoctor(new Doctor
            {
                FullName = f["FullName"],
                Specialty = f["Specialty"],
                LicenseNumber = f["LicenseNumber"],
                NationalId = f["NationalId"],
                Password = f["Password"],
                Email = f["Email"],
                OrganizationId = int.Parse(f["OrganizationId"])
            });
            return RedirectToAction("Dashboard", new { section = "doctors" });
        }

        [HttpPost]
        public IActionResult AddPharmacist(IFormCollection f)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.AddPharmacist(new Pharmacist { Name = f["Name"], Phone = f["Phone"], Email = f["Email"], NationalId = f["NationalId"], Password = f["Password"], LicenseNumber = f["LicenseNumber"] });
            return RedirectToAction("Dashboard", new { section = "pharmacy" });
        }

        [HttpPost]
        public IActionResult AddLabTech(IFormCollection f)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.AddLabTech(new LabTechnician
            {
                Name = f["Name"],
                Phone = f["Phone"],
                Email = f["Email"],
                NationalId = f["NationalId"],
                Password = f["Password"],
                Speciality = f["Speciality"]
            });
            return RedirectToAction("Dashboard", new { section = "lab" });
        }

        [HttpPost]
        public IActionResult AddRadiologist(IFormCollection f)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.AddRadiologist(new Radiologist
            {
                FullName = f["FullName"],
                Phone = f["Phone"],
                Email = f["Email"],
                NationalId = f["NationalId"],
                Specialty = f["Specialty"],
                Status = "Active",
                CreatedAt = DateTime.Now
            });
            return RedirectToAction("Dashboard", new { section = "radiology" });
        }

        [HttpPost]
        public IActionResult DeleteDoctor(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.DeleteDoctor(id);
            return RedirectToAction("Dashboard", new { section = "doctors" });
        }

        [HttpPost]
        public IActionResult DeletePharmacist(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.DeletePharmacist(id);
            return RedirectToAction("Dashboard", new { section = "pharmacy" });
        }

        [HttpPost]
        public IActionResult DeleteLabTech(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.DeleteLabTech(id);
            return RedirectToAction("Dashboard", new { section = "lab" });
        }

        [HttpPost]
        public IActionResult DeleteRadiologist(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _adminService.DeleteRadiologist(id);
            return RedirectToAction("Dashboard", new { section = "radiology" });
        }
    }
}