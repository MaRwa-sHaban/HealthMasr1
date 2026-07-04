using HealthMSR.DAL;
using HealthMSR.DAL.Enums;
using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class PharmacyService
    {
        private readonly AppDbContext _db;
        public PharmacyService(AppDbContext db) { _db = db; }

        public Pharmacist GetById(int id) => _db.Pharmacists.Find(id);
        
        public List<Prescription> GetPendingPrescriptions() =>
            _db.Prescriptions
                .Include(p => p.Items).ThenInclude(i => i.Medication)
                .Include(p => p.Dispenses)
                .Include(p => p.Encounter).ThenInclude(e => e.Patient)
                .Where(p => !p.Dispenses.Any(d => d.Status == DispenseStatus.Dispensed))
                .ToList();

        public List<Dispense> GetAllDispensed() =>
            _db.Dispenses
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Items)
                        .ThenInclude(i => i.Medication)
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Encounter)
                        .ThenInclude(e => e.Patient)
                .Include(d => d.Pharmacist)
                .Where(d => d.Status == DispenseStatus.Dispensed)
                .OrderByDescending(d => d.DispenseDate)
                .ToList();

        public void Dispense(int prescriptionId, int pharmacistId)
        {
            _db.Dispenses.Add(new Dispense
            {
                PrescriptionId = prescriptionId,
                PharmacistId = pharmacistId,
                DispenseDate = DateTime.Now,
                Status = DispenseStatus.Dispensed
            });
            _db.SaveChanges();
        }
        public int CountDispensedTodayByMe(int pharmacistId) =>
    _db.Dispenses.Count(d =>
        d.PharmacistId == pharmacistId &&
        d.Status == DispenseStatus.Dispensed &&
        d.DispenseDate.Date == DateTime.Today);

        public int CountDispensedByMe(int pharmacistId) =>
            _db.Dispenses.Count(d =>
                d.PharmacistId == pharmacistId &&
                d.Status == DispenseStatus.Dispensed);

        public int CountDispensedTodayAll() =>
            _db.Dispenses.Count(d =>
                d.Status == DispenseStatus.Dispensed &&
                d.DispenseDate.Date == DateTime.Today);

     
    }
}