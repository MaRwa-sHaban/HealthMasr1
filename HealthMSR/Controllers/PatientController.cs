using HealthMSR.BLL.Services;
//using HealthMSR.BLL.ViewModels;
using HealthMSR.DAL.Models;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult Dashboard(string section = "overview")
        {
            var patientId = HttpContext.Session.GetInt32("PatientId");
            if (patientId == null) return RedirectToAction("Login", "Auth");

            var patient = _patientService.GetById(patientId.Value);
            if (patient == null) return RedirectToAction("Login", "Auth");

            var vm = new PatientDashboardViewModel
            {
                Patient = patient,
                MedicalRecord = patient.MedicalRecord,
                ActiveSection = section,
                Encounters = patient.Encounters.OrderByDescending(e => e.StartTime).ToList(),
                Prescriptions = patient.Encounters.SelectMany(e => e.Prescriptions).OrderByDescending(p => p.DatePrescribed).ToList(),
                LabReports = patient.Encounters.SelectMany(e => e.LabReports).OrderByDescending(l => l.ReportId).ToList(),
                RadiologyReports = patient.Encounters.SelectMany(e => e.RadiologyReports).OrderByDescending(r => r.RequestedDate).ToList(),
                Observations = new List<Observation>()
            };

            return View(vm);
        }
    }
}