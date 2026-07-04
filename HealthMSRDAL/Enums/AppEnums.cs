namespace HealthMSR.DAL.Enums
{
    public enum Gender { Male, Female }
    public enum LabStatus { Pending, Completed }
    public enum RadiologyStatus { Pending, Ready }
    public enum DispenseStatus { Pending, Dispensed }
    public enum ConsentStatus { Active, Inactive }
    public enum UserRole { Patient, Doctor, Pharmacist, LabTechnician, Radiologist, Admin }
}