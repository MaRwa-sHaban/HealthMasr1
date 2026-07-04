using HealthMSR.DAL;
using HealthMSR.DAL.Models;
using HealthMSR.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class LabService
    {
        private readonly AppDbContext _db;
        public LabService(AppDbContext db) { _db = db; }

        public LabTechnician GetById(int id) => _db.LabTechnicians.Find(id);

        public List<Lab_Report> GetPendingRequests() =>
            _db.Lab_Reports
                .Include(l => l.Encounter).ThenInclude(e => e.Patient)
                .Where(l => l.Status == LabStatus.Pending)
                .ToList();

        public void UploadResult(int reportId, string result, int labTechId)
        {
            var report = _db.Lab_Reports.Find(reportId);
            if (report != null)
            {
                report.Results = result;
                report.Status = LabStatus.Completed;
                report.LabTechnicianId = labTechId;
                _db.SaveChanges();
            }
        }

        public int CountCompletedByMe(int labTechId) =>
            _db.Lab_Reports.Count(l =>
                l.Status == LabStatus.Completed &&
                l.LabTechnicianId == labTechId);
    }
}