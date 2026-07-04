using HealthMSR.BLL.Services;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HealthMSR.DAL.Enums;

namespace HealthMSR.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly PharmacyService _pharmacyService;
        private readonly PatientService _patientService;

        public PharmacyController(PharmacyService pharmacyService, PatientService patientService)
        {
            _pharmacyService = pharmacyService;
            _patientService = patientService;
        }

        public IActionResult Dashboard(string section = "overview", string searchId = null)
        {
            var pharmacistId = HttpContext.Session.GetInt32("PharmacistId");
            if (pharmacistId == null) return RedirectToAction("Login", "Auth");

            var pharmacist = _pharmacyService.GetById(pharmacistId.Value);
            if (pharmacist == null) return RedirectToAction("Login", "Auth");

            var pending = _pharmacyService.GetPendingPrescriptions();
            var dispensed = _pharmacyService.GetAllDispensed();
            var today = DateTime.Today;

            var vm = new PharmacyDashboardViewModel
            {
                Pharmacist = pharmacist,
                ActiveSection = section,
                TotalPendingAll = pending.Count,
                DispensedByMe = _pharmacyService.CountDispensedByMe(pharmacistId.Value),
                DispensedTodayAll = _pharmacyService.CountDispensedTodayAll(),
                DispensedTodayByMe = dispensed.Count(d =>
                    d.PharmacistId == pharmacistId.Value &&
                    d.DispenseDate.Date == today),

                //PendingPrescriptions = pending.Select(p => new PendingPrescriptionItem
                //{
                //    Prescription = p,
                //    PatientName = $"{p.Encounter?.Patient?.FirstName} {p.Encounter?.Patient?.LastName}",
                //    PatientNationalId = p.Encounter?.Patient?.NationalId,
                //    PatientId = p.Encounter?.PatientId ?? 0
                //}).ToList(),

                DispensedPrescriptions = dispensed.Select(d => new DispensedPrescriptionItem
                {
                    Prescription = d.Prescription,
                    PatientName = $"{d.Prescription?.Encounter?.Patient?.FirstName} {d.Prescription?.Encounter?.Patient?.LastName}",
                    PatientNationalId = d.Prescription?.Encounter?.Patient?.NationalId,
                    DispensedByName = d.Pharmacist?.Name ?? "—",
                    DispensedDate = d.DispenseDate
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
                    vm.PatientPrescriptions = patient.Encounters
                        .SelectMany(e => e.Prescriptions)
                        .OrderByDescending(p => p.DatePrescribed).ToList();
                }
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Dispense(int prescriptionId, string patientNationalId)
        {
            var pharmacistId = HttpContext.Session.GetInt32("PharmacistId");
            if (pharmacistId == null) return RedirectToAction("Login", "Auth");

            _pharmacyService.Dispense(prescriptionId, pharmacistId.Value);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patientNationalId });
        }
    }
}