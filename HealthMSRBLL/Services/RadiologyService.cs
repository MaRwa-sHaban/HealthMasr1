using HealthMSR.DAL;
using HealthMSR.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.BLL.Services
{
    public class RadiologyService
    {
        private readonly AppDbContext _db;
        public RadiologyService(AppDbContext db) { _db = db; }

        public Radiologist GetById(int id) => _db.Radiologists.Find(id);

        public List<Radiology_Report> GetPendingRequests() =>
            _db.Radiology_Reports
                .Include(r => r.Encounter).ThenInclude(e => e.Patient)
                .Where(r => r.Status == "Pending")
                .ToList();

        public int CountCompletedToday() =>
            _db.Radiology_Reports
                .Count(r => r.Status == "Ready" &&
                       r.ResultDate.HasValue &&
                       r.ResultDate.Value.Date == DateTime.Today);

        public async Task UploadResult(int reportId, string result,
            string notes, int radId, IFormFile scanFile, string webRootPath)
        {
            Console.WriteLine("========== UploadResult ==========");

            if (scanFile == null)
            {
                Console.WriteLine("scanFile = NULL");
            }
            else
            {
                Console.WriteLine("File Name: " + scanFile.FileName);
                Console.WriteLine("Length: " + scanFile.Length);
                Console.WriteLine("Content Type: " + scanFile.ContentType);
            }
            var report = _db.Radiology_Reports.Find(reportId);
            if (report == null) return;

            report.Result = result;
            report.Notes = notes;
            report.Status = "Ready";
            report.ResultDate = DateTime.Now;
            report.RadiologistId = radId;

            if (scanFile != null && scanFile.Length > 0)
            {
                var allowed = new[] { "application/pdf", "image/jpeg", "image/png" };
                if (allowed.Contains(scanFile.ContentType))
                {
                    var uploadPath = Path.Combine(webRootPath, "uploads", "radiology");
                    Directory.CreateDirectory(uploadPath);
                    var fileName = $"rad_{reportId}_{DateTime.Now.Ticks}{Path.GetExtension(scanFile.FileName)}";
                    using var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create);
                    await scanFile.CopyToAsync(stream);
                    report.ImageUrl = $"/uploads/radiology/{fileName}";
                }
            }
            _db.SaveChanges();
        }
    }
}