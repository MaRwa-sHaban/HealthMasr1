using HealthMSR.DAL;
using HealthMSR.DAL.Enums;
using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
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

        public Encounter AddEncounter(int patientId, int doctorId,
            string status, DateTime startTime)
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
            return encounter;
        }
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

        public void AddPrescription(int encounterId, string notes,
            List<string> medNames, List<string> dosages,
            List<string> frequencies, List<string> durations)
        {
            var prescription = new Prescription
            {
                EncounterId = encounterId,
                DatePrescribed = DateTime.Now,
                Notes = notes ?? string.Empty,
                Items = new List<Prescription_Item>()
            };

            for (int i = 0; i < medNames.Count; i++)
            {
                if (string.IsNullOrEmpty(medNames[i])) continue;
                var med = _db.Medications.FirstOrDefault(m => m.DrugName == medNames[i]);
                if (med == null)
                {
                    med = new Medication { DrugName = medNames[i], Route = "Oral" };
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

        public void AddLabRequest(int encounterId, int patientId,
            string testName, int labTechId = 1)
        {
            _db.Lab_Reports.Add(new Lab_Report
            {
                EncounterId = encounterId,
                PatientId = patientId,
                TestName = testName,
                Status = LabStatus.Pending,
                Results = "—",
                LabTechnicianId = labTechId
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
                if (!string.IsNullOrWhiteSpace(v.Value))
                    _db.Observations.Add(new Observation
                    {
                        Value = v.Value,
                        Unit = v.Key,
                        DateTime = DateTime.Now,
                        PractitionerId = doctorId,
                        SystemId = systemId
                    });

            _db.SaveChanges();
        }

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

        //public List<Observation> GetPatientObservations(int value)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<Medication> GetMedications()
        //{
        //    throw new NotImplementedException();
        //}

        //public object SearchPatientById(int patientId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}