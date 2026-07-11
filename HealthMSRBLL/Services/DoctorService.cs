using HealthMSR.DAL;
using HealthMSR.DAL.Models;
using HealthMSR.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class DoctorService
    {
        private readonly AppDbContext _db;
        public DoctorService(AppDbContext db) { _db = db; }

        public Doctor GetById(int id) =>
            _db.Doctors
                .Include(d => d.Organization)
                .FirstOrDefault(d => d.PractitionerId == id);

        public Doctor GetByNationalId(string nationalId) =>
            _db.Doctors.FirstOrDefault(d =>
                d.NationalId == nationalId ||
                d.LicenseNumber == nationalId);

        public Patient SearchPatient(string nationalId) =>
            _db.Patients
                .Include(p => p.MedicalRecord)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.Prescriptions)
                        .ThenInclude(p => p.Items)
                            .ThenInclude(i => i.Medication)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.Prescriptions)
                        .ThenInclude(p => p.Dispenses)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.LabReports)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.RadiologyReports)
                .FirstOrDefault(p => p.NationalId == nationalId);

        public Patient SearchPatientById(int patientId) =>
            _db.Patients
                .Include(p => p.MedicalRecord)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.Prescriptions)
                        .ThenInclude(p => p.Items)
                            .ThenInclude(i => i.Medication)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.Prescriptions)
                        .ThenInclude(p => p.Dispenses)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.LabReports)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.RadiologyReports)
                .FirstOrDefault(p => p.PatientId == patientId);

        public List<Medication> GetMedications() => _db.Medications.ToList();

        public List<Observation> GetPatientObservations(int doctorId) =>
            _db.Observations
                .Include(o => o.Doctor)
                .Where(o => o.PractitionerId == doctorId)
                .OrderByDescending(o => o.DateTime)
                .ToList();

        public int GetTodayPatients(int doctorId) =>
            _db.Encounters.Count(e =>
                e.PractitionerId == doctorId &&
                e.StartTime.Date == DateTime.Today);

        public List<Lab_Report> GetMyLabRequests(int doctorId) =>
            _db.Lab_Reports
                .Include(l => l.Encounter).ThenInclude(e => e.Patient)
                .Where(l => l.Encounter.PractitionerId == doctorId)
                .OrderByDescending(l => l.ReportId)
                .ToList();

        public List<Radiology_Report> GetMyRadRequests(int doctorId) =>
            _db.Radiology_Reports
                .Include(r => r.Encounter).ThenInclude(e => e.Patient)
                .Where(r => r.Encounter.PractitionerId == doctorId)
                .OrderByDescending(r => r.RequestedDate)
                .ToList();

        // ===== SAVE FULL VISIT =====
        public void SaveVisit(SaveVisitModel model)
        {
            // 1. Create Encounter
            var encounter = new Encounter
            {
                PatientId = model.PatientId,
                PractitionerId = model.DoctorId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Status = string.IsNullOrEmpty(model.Notes)
                    ? model.Diagnosis
                    : $"{model.Diagnosis} | {model.Notes}"
            };
            _db.Encounters.Add(encounter);
            _db.SaveChanges();

            // 2. Vital Signs
            if (!string.IsNullOrWhiteSpace(model.BP) ||
                !string.IsNullOrWhiteSpace(model.Sugar) ||
                !string.IsNullOrWhiteSpace(model.Temp) ||
                !string.IsNullOrWhiteSpace(model.Pulse))
            {
                var system = _db.External_Systems.FirstOrDefault();
                if (system == null)
                {
                    system = new External_System
                    {
                        SystemName = "HealthMSR",
                        SystemType = "Internal",
                        AuthMethod = "Session"
                    };
                    _db.External_Systems.Add(system);
                    _db.SaveChanges();
                }

                var vitals = new Dictionary<string, string>
                {
                    { "Blood Pressure (mmHg)", model.BP ?? "" },
                    { "Blood Sugar (mg/dL)", model.Sugar ?? "" },
                    { "Temperature (°C)", model.Temp ?? "" },
                    { "Pulse (bpm)", model.Pulse ?? "" }
                };

                foreach (var v in vitals)
                    if (!string.IsNullOrWhiteSpace(v.Value))
                        _db.Observations.Add(new Observation
                        {
                            Value = v.Value,
                            Unit = v.Key,
                            DateTime = DateTime.Now,
                            PractitionerId = model.DoctorId,
                            SystemId = system.SystemId
                        });

                _db.SaveChanges();
            }

            // 3. Prescription
            var validMeds = model.Medications?
                .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                .ToList();

            if (validMeds != null && validMeds.Any())
            {
                var prescription = new Prescription
                {
                    EncounterId = encounter.EncounterId,
                    DatePrescribed = DateTime.Now,
                    Notes = model.PrescriptionNotes ?? "",
                    Items = new List<Prescription_Item>()
                };

                foreach (var med in validMeds)
                {
                    var medication = _db.Medications
                        .FirstOrDefault(m => m.DrugName == med.Name);
                    if (medication == null)
                    {
                        medication = new Medication
                        {
                            DrugName = med.Name,
                            Route = "Oral"
                        };
                        _db.Medications.Add(medication);
                        _db.SaveChanges();
                    }

                    prescription.Items.Add(new Prescription_Item
                    {
                        MedicationId = medication.MedicationId,
                        Dosage = med.Dosage ?? "—",
                        Frequency = med.Frequency ?? "Once daily",
                        Duration = med.Duration ?? "7 days"
                    });
                }

                _db.Prescriptions.Add(prescription);
                _db.SaveChanges();
            }

            // 4. Lab Tests
            var validLabs = model.LabTests?
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            if (validLabs != null && validLabs.Any())
            {
                foreach (var test in validLabs)
                {
                    _db.Lab_Reports.Add(new Lab_Report
                    {
                        EncounterId = encounter.EncounterId,
                        PatientId = model.PatientId,
                        TestName = test,
                        Status = LabStatus.Pending,
                        Results = "—",
                        LabTechnicianId = 1
                    });
                }
                _db.SaveChanges();
            }

            // 5. Radiology Scans
            var validScans = model.RadiologyScans?
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (validScans != null && validScans.Any())
            {
                foreach (var scan in validScans)
                {
                    _db.Radiology_Reports.Add(new Radiology_Report
                    {
                        EncounterId = encounter.EncounterId,
                        ScanType = scan,
                        Status = "Pending",
                        RequestedDate = DateTime.Now
                    });
                }
                _db.SaveChanges();
            }
        }

        public void AddVitalSigns(int doctorId, int systemId,
      string bp, string sugar, string temp, string pulse)
        {
            var vitals = new Dictionary<string, string>
    {
        { "Blood Pressure (mmHg)", bp },
        { "Blood Sugar (mg/dL)", sugar },
        { "Temperature (°C)", temp },
        { "Pulse (bpm)", pulse }
    };

            foreach (var v in vitals)
            {
                if (!string.IsNullOrWhiteSpace(v.Value))
                {
                    _db.Observations.Add(new Observation
                    {
                        Value = v.Value,
                        Unit = v.Key,
                        DateTime = DateTime.Now,
                        PractitionerId = doctorId,
                        SystemId = systemId
                    });
                }
            }

            _db.SaveChanges();
        }

        public void AddLabRequest(int encounterId, int patientId, string testName)
        {
            _db.Lab_Reports.Add(new Lab_Report
            {
                EncounterId = encounterId,
                PatientId = patientId,
                TestName = testName,
                Status = LabStatus.Pending,
                Results = "—",
                LabTechnicianId = 1
            });

            _db.SaveChanges();
        }

        public void AddRadRequest(int encounterId, string scanType)
        {
            _db.Radiology_Reports.Add(new Radiology_Report
            {
                EncounterId = encounterId,
                ScanType = scanType,
                Status = "Pending",
                RequestedDate = DateTime.Now
            });

            _db.SaveChanges();
        }

        public void AddEncounter(int patientId, int doctorId, string status, DateTime startTime)
        {
            var encounter = new Encounter
            {
                PatientId = patientId,
                PractitionerId = doctorId,
                StartTime = startTime,
                EndTime = startTime.AddHours(1),
                Status = status
            };

            _db.Encounters.Add(encounter);
            _db.SaveChanges();
        }

        public void AddPrescription(int encounterId, string notes,
      List<string> medicationIds,
      List<string> dosages,
      List<string> frequencies,
      List<string> durations)
        {
            var prescription = new Prescription
            {
                EncounterId = encounterId,
                DatePrescribed = DateTime.Now,
                Notes = notes ?? "",
                Items = new List<Prescription_Item>()
            };

            for (int i = 0; i < medicationIds.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(medicationIds[i]))
                    continue;

                var med = _db.Medications
                    .FirstOrDefault(m => m.DrugName == medicationIds[i]);

                if (med == null)
                {
                    med = new Medication
                    {
                        DrugName = medicationIds[i],
                        Route = "Oral"
                    };

                    _db.Medications.Add(med);
                    _db.SaveChanges();
                }

                prescription.Items.Add(new Prescription_Item
                {
                    MedicationId = med.MedicationId,
                    Dosage = dosages.ElementAtOrDefault(i) ?? "—",
                    Frequency = frequencies.ElementAtOrDefault(i) ?? "Once daily",
                    Duration = durations.ElementAtOrDefault(i) ?? "7 days"
                });
            }

            _db.Prescriptions.Add(prescription);
            _db.SaveChanges();
        }
    }

    // Model للـ SaveVisit
    public class SaveVisitModel
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Vitals
        public string? BP { get; set; }
        public string? Sugar { get; set; }
        public string? Temp { get; set; }
        public string? Pulse { get; set; }

        // Prescription
        public string? PrescriptionNotes { get; set; }
        public List<MedicationItem>? Medications { get; set; }

        // Labs
        public List<string>? LabTests { get; set; }

        // Radiology
        public List<string>? RadiologyScans { get; set; }
    }

    public class MedicationItem
    {
        public string Name { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
    }
}