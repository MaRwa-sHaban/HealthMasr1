using HealthMSR.BLL.Services;
//using HealthMSR.BLL.ViewModels;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class LabController : Controller
    {
        private readonly LabService _labService;
        private readonly PatientService _patientService;

        public LabController(LabService labService, PatientService patientService)
        {
            _labService = labService;
            _patientService = patientService;
        }

        public IActionResult Dashboard(string section = "overview", string searchId = null)
        {
            var labTechId = HttpContext.Session.GetInt32("LabTechId");
            if (labTechId == null) return RedirectToAction("Login", "Auth");

            var labTech = _labService.GetById(labTechId.Value);
            if (labTech == null) return RedirectToAction("Login", "Auth");

            var pending = _labService.GetPendingRequests();

            var vm = new LabDashboardViewModel
            {
                LabTechnician = labTech,
                ActiveSection = section,
                CompletedToday = _labService.CountCompletedByMe(labTechId.Value),
                PendingLabs = pending.Select(l => new PendingLabItem
                {
                    LabReport = l,
                    PatientName = $"{l.Encounter?.Patient?.FirstName} {l.Encounter?.Patient?.LastName}",
                    PatientNationalId = l.Encounter?.Patient?.NationalId
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
                    vm.PatientLabReports = patient.Encounters
                        .SelectMany(e => e.LabReports)
                        .OrderByDescending(l => l.ReportId).ToList();
                }
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult UploadResult(int reportId, string result, string patientNationalId)
        {
            var labTechId = HttpContext.Session.GetInt32("LabTechId");
            if (labTechId == null) return RedirectToAction("Login", "Auth");

            _labService.UploadResult(reportId, result, labTechId.Value);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patientNationalId });
        }
    }
}