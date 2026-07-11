using HealthMSR.BLL.Services;
//using HealthMSR.BLL.ViewModels;
using HealthMSR.DAL.Models;
using HealthMSR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public IActionResult Dashboard(string section = "overview", string searchId = null)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            var doctor = _doctorService.GetById(doctorId.Value);
            if (doctor == null) return RedirectToAction("Login", "Auth");

            var vm = new DoctorDashboardViewModel
            {
                Doctor = doctor,
                ActiveSection = section,
                TodayPatients = _doctorService.GetTodayPatients(doctorId.Value),
                Age = doctor.NationalId?.Length >= 3
                    ? DateTime.Now.Year - (doctor.NationalId[0] == '2' ? 1900 : 2000)
                      - int.Parse(doctor.NationalId.Substring(1, 2)) : 0
            };

            var labReqs = _doctorService.GetMyLabRequests(doctorId.Value);
            var radReqs = _doctorService.GetMyRadRequests(doctorId.Value);

            vm.MyLabRequests = labReqs.Select(l => new LabRequestItem
            {
                LabReport = l,
                PatientName = $"{l.Encounter?.Patient?.FirstName} {l.Encounter?.Patient?.LastName}",
                PatientNationalId = l.Encounter?.Patient?.NationalId
            }).ToList();

            vm.MyRadRequests = radReqs.Select(static r => new RadRequestItem
            {
                RadReport = r,
                PatientName = $"{r.Encounter?.Patient?.FirstName} {r.Encounter?.Patient?.LastName}",
                PatientNationalId = r.Encounter?.Patient?.NationalId
            }).ToList();

            if (!string.IsNullOrEmpty(searchId))
            {
                var patient = _doctorService.SearchPatient(searchId);
                if (patient == null)
                    vm.SearchError = "Patient not found.";
                else
                {
                    vm.SearchedPatient = patient;
                    vm.PatientEncounters = patient.Encounters.OrderByDescending(e => e.StartTime).ToList();
                    vm.PatientPrescriptions = patient.Encounters.SelectMany(e => e.Prescriptions).OrderByDescending(p => p.DatePrescribed).ToList();
                    vm.PatientLabReports = patient.Encounters.SelectMany(e => e.LabReports).OrderByDescending(l => l.ReportId).ToList();
                    vm.PatientRadReports = patient.Encounters.SelectMany(e => e.RadiologyReports).OrderByDescending(r => r.RequestedDate).ToList();
                    vm.PatientObservations = _doctorService.GetPatientObservations(doctorId.Value);
                    vm.Medications = _doctorService.GetMedications();
                }
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddEncounter(int patientId, string status, DateTime startTime, string notes)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            _doctorService.AddEncounter(patientId, doctorId.Value,
                string.IsNullOrEmpty(notes) ? status : $"{status} | {notes}", startTime);

            var patient = _doctorService.SearchPatient(patientId.ToString());
            return RedirectToAction("Dashboard", new { section = "search", searchId = patient?.NationalId });
        }

        [HttpPost]
        public IActionResult AddPrescription(int patientId, int encounterId, string notes,
            List<string> medicationIds, List<string> dosages,
            List<string> frequencies, List<string> durations)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            _doctorService.AddPrescription(encounterId, notes, medicationIds, dosages, frequencies, durations);

            var doctor = _doctorService.GetById(doctorId.Value);
            var patient = _doctorService.SearchPatientById(patientId);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patient?.NationalId });
        }

        [HttpPost]
        public IActionResult AddLabRequest(int patientId, int encounterId, string testName)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            _doctorService.AddLabRequest(encounterId, patientId, testName);

            var patient = _doctorService.SearchPatientById(patientId);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patient?.NationalId });
        }

        [HttpPost]
        public IActionResult AddRadRequest(int patientId, int encounterId, string scanType)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            _doctorService.AddRadRequest(encounterId, scanType);

            var patient = _doctorService.SearchPatientById(patientId);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patient?.NationalId });
        }

        [HttpPost]
        public IActionResult AddVitalSigns(int patientId, int encounterId,
            string bp, string sugar, string temp, string pulse)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            _doctorService.AddVitalSigns(doctorId.Value, 1, bp, sugar, temp, pulse);

            var patient = _doctorService.SearchPatientById(patientId);
            return RedirectToAction("Dashboard", new { section = "search", searchId = patient?.NationalId });
        }
        [HttpPost]
        public IActionResult SaveVisit(IFormCollection form)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            var patientId = int.Parse(form["PatientId"]);

            // Medications
            var medNames = form["medName"].ToList();
            var medDosages = form["medDosage"].ToList();
            var medFreqs = form["medFreq"].ToList();
            var medDurations = form["medDuration"].ToList();

            var medications = new List<MedicationItem>();
            for (int i = 0; i < medNames.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(medNames[i]))
                {
                    medications.Add(new MedicationItem
                    {
                        Name = medNames[i],
                        Dosage = medDosages.ElementAtOrDefault(i) ?? "—",
                        Frequency = medFreqs.ElementAtOrDefault(i) ?? "Once daily",
                        Duration = medDurations.ElementAtOrDefault(i) ?? "7 days"
                    });
                }
            }

            // Lab Tests
            var labTests = form["labTest"].Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

            // Radiology Scans
            var radScans = form["radScan"].Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            var model = new SaveVisitModel
            {
                PatientId = patientId,
                DoctorId = doctorId.Value,
                Diagnosis = form["Diagnosis"],
                Notes = form["Notes"],
                BP = form["BP"],
                Sugar = form["Sugar"],
                Temp = form["Temp"],
                Pulse = form["Pulse"],
                PrescriptionNotes = form["PrescriptionNotes"],
                Medications = medications,
                LabTests = labTests,
                RadiologyScans = radScans
            };

            _doctorService.SaveVisit(model);

            var patient = _doctorService.SearchPatientById(patientId);
            return RedirectToAction("Dashboard", new
            {
                section = "search",
                searchId = patient?.NationalId
            });
        }

        public IActionResult PatientProfile(int patientId)
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            var patient = _doctorService.SearchPatientById(patientId);
            if (patient == null) return RedirectToAction("Dashboard");

            var vm = new PatientDashboardViewModel
            {
                Patient = (Patient)patient,
                MedicalRecord = patient.MedicalRecord,
                ActiveSection = "records",
                Encounters = patient.Encounters.OrderByDescending(e => e.StartTime).ToList(),
                Prescriptions = patient.Encounters.SelectMany(e => e.Prescriptions).OrderByDescending(p => p.DatePrescribed).ToList(),
                LabReports = patient.Encounters.SelectMany(e => e.LabReports).OrderByDescending(l => l.ReportId).ToList(),
                RadiologyReports = patient.Encounters.SelectMany(e => e.RadiologyReports).OrderByDescending(r => r.RequestedDate).ToList(),
                Observations = new List<Observation>()
            };

            return View("~/Views/Patient/Dashboard.cshtml", vm);
        }
    }

    public class RadRequestItem
    {
        public Radiology_Report RadReport { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
    }

    public class LabRequestItem
    {
        public Lab_Report LabReport { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
    }
}