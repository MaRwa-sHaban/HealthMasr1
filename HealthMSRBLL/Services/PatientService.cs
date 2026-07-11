using HealthMSR.DAL;
using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class PatientService
    {
        private readonly AppDbContext _db;
        public PatientService(AppDbContext db) { _db = db; }

        public Patient GetById(int id) =>
            _db.Patients
                .Include(p => p.MedicalRecord)
                .Include(p => p.Encounters)
                    .ThenInclude(e => e.Doctor)
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
                .FirstOrDefault(p => p.PatientId == id);

        //public Patient GetByNationalId(string nationalId) =>
        //    _db.Patients
        //        .Include(p => p.MedicalRecord)
        //        .FirstOrDefault(p => p.NationalId == nationalId);
        public Patient GetByNationalId(string nationalId) =>
    _db.Patients
        .Include(p => p.MedicalRecord)
        .Include(p => p.Encounters)
            .ThenInclude(e => e.Doctor)
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
        public Patient Register(Patient patient, string bloodType)
        {
            _db.Patients.Add(patient);
            _db.SaveChanges();

            _db.Medical_Records.Add(new Medical_Record
            {
                PatientId = patient.PatientId,
                BloodType = bloodType,
                DateCreated = DateTime.Now,
                GlobalStatus = "Active"
            });
            _db.SaveChanges();
            return patient;
        }

        public List<Patient> GetAll() =>
            _db.Patients.Include(p => p.MedicalRecord).ToList();
    }
}