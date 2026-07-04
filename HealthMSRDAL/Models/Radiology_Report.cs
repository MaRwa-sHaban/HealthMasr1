namespace HealthMSR.DAL.Models
{
    public class Radiology_Report
    {
        public int Id { get; set; }
        public string ScanType { get; set; }
        public string Status { get; set; }
        public string? Result { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ResultDate { get; set; }

        public int EncounterId { get; set; }
        public Encounter Encounter { get; set; }

        public int? RadiologistId { get; set; }
        public Radiologist? Radiologist { get; set; }
    }
}