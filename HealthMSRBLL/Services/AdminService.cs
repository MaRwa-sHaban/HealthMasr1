using HealthMSR.DAL;
using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class AdminService
    {
        private readonly AppDbContext _db;
        public AdminService(AppDbContext db) { _db = db; }

        public List<Doctor> GetAllDoctors(string specialty = null, string search = null)
        {
            var query = _db.Doctors.Include(d => d.Organization).AsQueryable();
            if (!string.IsNullOrEmpty(specialty))
                query = query.Where(d => d.Specialty == specialty);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(d => d.FullName.Contains(search));
            return query.ToList();
        }

        public List<Pharmacist> GetAllPharmacists() => _db.Pharmacists.ToList();
        public List<LabTechnician> GetAllLabTechs() => _db.LabTechnicians.ToList();
        public List<Radiologist> GetAllRadiologists() => _db.Radiologists.ToList();
        public List<Patient> GetAllPatients() =>
            _db.Patients.Include(p => p.MedicalRecord).ToList();
        public List<Organization> GetAllOrganizations() => _db.Organizations.ToList();

        public int CountPatients() => _db.Patients.Count();
        public int CountDoctors() => _db.Doctors.Count();
        public int CountPharmacists() => _db.Pharmacists.Count();
        public int CountLabTechs() => _db.LabTechnicians.Count();
        public int CountRadiologists() => _db.Radiologists.Count();

        public void AddDoctor(Doctor doctor) { _db.Doctors.Add(doctor); _db.SaveChanges(); }
        public void AddPharmacist(Pharmacist ph) { _db.Pharmacists.Add(ph); _db.SaveChanges(); }
        public void AddLabTech(LabTechnician lt) { _db.LabTechnicians.Add(lt); _db.SaveChanges(); }
        public void AddRadiologist(Radiologist rad) { _db.Radiologists.Add(rad); _db.SaveChanges(); }

        public void DeleteDoctor(int id)
        {
            var doc = _db.Doctors.Find(id);
            if (doc != null) { _db.Doctors.Remove(doc); _db.SaveChanges(); }
        }
        public void DeletePharmacist(int id)
        {
            var ph = _db.Pharmacists.Find(id);
            if (ph != null) { _db.Pharmacists.Remove(ph); _db.SaveChanges(); }
        }
        public void DeleteLabTech(int id)
        {
            var lt = _db.LabTechnicians.Find(id);
            if (lt != null) { _db.LabTechnicians.Remove(lt); _db.SaveChanges(); }
        }
        public void DeleteRadiologist(int id)
        {
            var rad = _db.Radiologists.Find(id);
            if (rad != null) { _db.Radiologists.Remove(rad); _db.SaveChanges(); }
        }
    }
}