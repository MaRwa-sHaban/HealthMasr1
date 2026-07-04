using HealthMSR.BLL.Services;
//using HealthMSR.BLL.ViewModels;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class RadiologyController : Controller
    {
        private readonly RadiologyService _radiologyService;
        private readonly PatientService _patientService;
        private readonly IWebHostEnvironment _env;

        public RadiologyController(RadiologyService radiologyService,
            PatientService patientService, IWebHostEnvironment env)
        {
            _radiologyService = radiologyService;
            _patientService = patientService;
            _env = env;
        }

        public IActionResult Dashboard(string section = "overview", string searchId = null)
        {
            var radId = HttpContext.Session.GetInt32("RadiologistId");
            if (radId == null) return RedirectToAction("Login", "Auth");

            var radiologist = _radiologyService.GetById(radId.Value);
            if (radiologist == null) return RedirectToAction("Login", "Auth");

            var pending = _radiologyService.GetPendingRequests();

            var vm = new RadiologyDashboardViewModel
            {
                Radiologist = radiologist,
                ActiveSection = section,
                CompletedToday = _radiologyService.CountCompletedToday(),
                PendingScans = pending.Select(r => new PendingRadItem
                {
                    RadReport = r,
                    PatientName = $"{r.Encounter?.Patient?.FirstName} {r.Encounter?.Patient?.LastName}",
                    PatientNationalId = r.Encounter?.Patient?.NationalId
                }).ToList()
            };

            if (!string.IsNullOrEmpty(searchId))
            {
                var patient = _patientService.GetByNationalId(searchId);
                if (patient == null)
                    vm.SearchError = "Patient not found.";
                else
                {
                    vm.SearchedPatient = patient;
                    vm.PatientRadReports = patient.Encounters
                        .SelectMany(e => e.RadiologyReports)
                        .OrderByDescending(r => r.RequestedDate).ToList();
                }
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadResult(int reportId, string result,
            string notes, string patientNationalId, BLL.Services.IFormFile scanFile)
        {
            var radId = HttpContext.Session.GetInt32("RadiologistId");
            if (radId == null) return RedirectToAction("Login", "Auth");

            await _radiologyService.UploadResult(reportId, result, notes,
                radId.Value, scanFile, _env.WebRootPath);

            return RedirectToAction("Dashboard", new { section = "search", searchId = patientNationalId });
        }
    }
}