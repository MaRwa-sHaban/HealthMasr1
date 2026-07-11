using HealthMSR.DAL.Enums;
using HealthMSR.DAL.Models;
using HealthMSR.BLL.Services;
using HealthMSR.DAL;
using Microsoft.AspNetCore.Mvc;

namespace HealthMSR.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly HealthMSR.BLL.Services.EmailService _emailService;

        public AuthController(AppDbContext db, HealthMSR.BLL.Services.EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email, string role)
        {
            // بندور على الإيميل حسب الـ Role
            string foundEmail = null;

            if (role == "Patient")
                foundEmail = _db.Patients.FirstOrDefault(p => p.Email == email)?.Email;
            else if (role == "Doctor")
                foundEmail = _db.Doctors.FirstOrDefault(d => d.Email == email)?.FullName != null ? email : null;
            else if (role == "Pharmacy")
                foundEmail = _db.Pharmacists.FirstOrDefault(p => p.Email == email)?.Email;
            else if (role == "Lab")
                foundEmail = _db.LabTechnicians.FirstOrDefault(l => l.Email == email)?.Email;
            else if (role == "Radiology")
                foundEmail = _db.Radiologists.FirstOrDefault(r => r.Email == email)?.Email;

            if (foundEmail != null)
            {
                var token = Guid.NewGuid().ToString();
                _db.PasswordResetTokens.Add(new PasswordResetToken
                {
                    Email = email,
                    Token = token,
                    Role = role,
                    ExpiresAt = DateTime.Now.AddMinutes(30),
                    IsUsed = false
                });
                _db.SaveChanges();

                var resetLink = Url.Action("ResetPassword", "Auth",
                    new { token, email, role }, Request.Scheme);
                await _emailService.SendResetEmail(email, resetLink);
            }

            ViewBag.Message = "If this email exists, a reset link has been sent.";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email, string role)
        {
            var record = _db.PasswordResetTokens.FirstOrDefault(t =>
                t.Token == token && t.Email == email && !t.IsUsed &&
                t.ExpiresAt > DateTime.Now);

            if (record == null)
            {
                ViewBag.Error = "Invalid or expired reset link.";
                return View();
            }

            ViewBag.Token = token;
            ViewBag.Email = email;
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string email, string role,
            string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords don't match.";
                return View();
            }

            var record = _db.PasswordResetTokens.FirstOrDefault(t =>
                t.Token == token && t.Email == email && !t.IsUsed &&
                t.ExpiresAt > DateTime.Now);

            if (record == null)
            {
                ViewBag.Error = "Invalid or expired link.";
                return View();
            }

        
            if (role == "Patient")
            {
                var u = _db.Patients.FirstOrDefault(p => p.Email == email);
                if (u != null) { /* Patient مش عنده Password field حالياً */ }
            }
            else if (role == "Doctor")
            {
                var u = _db.Doctors.FirstOrDefault(d => d.Email == email);
                if (u != null) { u.Password = newPassword; }
            }
            else if (role == "Pharmacy")
            {
                var u = _db.Pharmacists.FirstOrDefault(p => p.Email == email);
                if (u != null) { u.Password = newPassword; }
            }
            else if (role == "Lab")
            {
                var u = _db.LabTechnicians.FirstOrDefault(l => l.Email == email);
                if (u != null) { u.Password = newPassword; }
            }
            else if (role == "Radiology")
            {
                var u = _db.Radiologists.FirstOrDefault(r => r.Email == email);
                if (u != null) { u.PasswordHash = newPassword; }
            }

            record.IsUsed = true;
            _db.SaveChanges();

            return RedirectToAction("Login");
        }
        // ===== LOGIN GET =====
        [HttpGet]
        public IActionResult Login() => View();

        // ===== LOGIN POST =====
        [HttpPost]
        public IActionResult Login(IFormCollection form)
        {
            var nationalId = form["NationalId"].ToString().Trim();
            var password = form["Password"].ToString().Trim();
            var role = form["Role"].ToString().Trim();

            switch (role)
            {
                case "Patient":
                    var patient = _db.Patients.FirstOrDefault(p => p.NationalId == nationalId);
                    if (patient != null)
                    {
                        HttpContext.Session.SetInt32("PatientId", patient.PatientId);
                        HttpContext.Session.SetString("Role", "Patient");
                        return RedirectToAction("Dashboard", "Patient");
                    }
                    break;

                case "Doctor":
                    var doctor = _db.Doctors.FirstOrDefault(d =>
         (d.NationalId == nationalId || d.LicenseNumber == nationalId) &&
         (string.IsNullOrEmpty(d.Password) || d.Password == password));
                    if (doctor != null)
                    {
                        HttpContext.Session.SetInt32("DoctorId", doctor.PractitionerId);
                        HttpContext.Session.SetString("Role", "Doctor");
                        HttpContext.Session.SetString("DoctorName", doctor.FullName);
                        return RedirectToAction("Dashboard", "Doctor");
                    }
                    break;

                case "Pharmacy":
                    var pharmacist = _db.Pharmacists.FirstOrDefault(p =>
           p.NationalId == nationalId &&
           (string.IsNullOrEmpty(p.Password) || p.Password == password));
                    if (pharmacist != null)
                    {
                        HttpContext.Session.SetInt32("PharmacistId", pharmacist.Id);
                        HttpContext.Session.SetString("Role", "Pharmacy");
                        return RedirectToAction("Dashboard", "Pharmacy");
                    }
                    break;

                case "Lab":
                    var labTech = _db.LabTechnicians.FirstOrDefault(l =>
           l.NationalId == nationalId &&
           (string.IsNullOrEmpty(l.Password) || l.Password == password));
                    if (labTech != null)
                    {
                        HttpContext.Session.SetInt32("LabTechId", labTech.Id);
                        HttpContext.Session.SetString("Role", "Lab");
                        return RedirectToAction("Dashboard", "Lab");
                    }
                    break;

                case "Radiology":
                    var rad = _db.Radiologists.FirstOrDefault(r => r.NationalId == nationalId);
                    if (rad != null)
                    {
                        HttpContext.Session.SetInt32("RadiologistId", rad.Id);
                        HttpContext.Session.SetString("Role", "Radiology");
                        return RedirectToAction("Dashboard", "Radiology");
                    }
                    break;

                case "Admin":
                    if (nationalId == "admin" && password == "admin123")
                    {
                        HttpContext.Session.SetString("Role", "Admin");
                        HttpContext.Session.SetString("AdminName", "Admin");
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    break;
            }

            ViewBag.Error = "Invalid credentials. Please try again.";
            return View();
        }

        // ===== REGISTER GET =====
        [HttpGet]
        public IActionResult Register() => View();

        // ===== REGISTER POST =====
        [HttpPost]
        public IActionResult Register(IFormCollection form)
        {
            var role = form["Role"].ToString().Trim();
            var nationalId = form["NationalId"].ToString().Trim();
            var fullName = form["FullName"].ToString().Trim();
            var nameParts = fullName.Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";
            var password = form["Password"].ToString().Trim();
            var confirm = form["ConfirmPassword"].ToString().Trim();

            // Validation
            if (string.IsNullOrEmpty(nationalId) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View();
            }

            if (password != confirm)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }
            // ===Admin===
            if (role == "Admin")
            {
                ViewBag.Error = "Admin access is restricted. Please use the login page.";
                return View();
            }
            // ===== PATIENT =====
            if (role == "Patient")
            {
                if (_db.Patients.Any(p => p.NationalId == nationalId))
                {
                    ViewBag.Error = "This National ID is already registered.";
                    return View();
                }

                var patient = new Patient
                {
                    NationalId = nationalId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = form["Email"],
                    Phone = form["Phone"],
                    DateOfBirth = DateTime.TryParse(form["DateOfBirth"], out var dob) ? dob : DateTime.Now.AddYears(-25),
                    Gender = form["Gender"] == "0" ? Gender.Male : Gender.Female
                };

                _db.Patients.Add(patient);
                _db.SaveChanges();

                var record = new Medical_Record
                {
                    PatientId = patient.PatientId,
                    BloodType = form["BloodType"].ToString().Length > 0 ? form["BloodType"].ToString() : "—",
                    DateCreated = DateTime.Now,
                    GlobalStatus = "Active"
                };
                _db.Medical_Records.Add(record);
                _db.SaveChanges();

                HttpContext.Session.SetInt32("PatientId", patient.PatientId);
                HttpContext.Session.SetString("Role", "Patient");
                return RedirectToAction("Dashboard", "Patient");
            }

            // ===== DOCTOR =====
            if (role == "Doctor")
            {
                try
                {
                    // Organization
                    var org = _db.Organizations.FirstOrDefault();
                    if (org == null)
                    {
                        org = new Organization
                        {
                            Name = "HealthMSR Hospital",
                            OrgType = "Hospital",
                            Address = "Cairo",
                            TaxId = "000000000"
                        };
                        _db.Organizations.Add(org);
                        _db.SaveChanges();
                    }

                    // Doctor
                    var specialty = form["Specialty"].ToString();
                    if (string.IsNullOrWhiteSpace(specialty)) specialty = "General Practice";

                    var doctor = new Doctor
                    {
                        FullName = fullName ?? string.Empty,
                        Email = form["Email"].ToString(),
                        Specialty = !string.IsNullOrEmpty(form["Specialty"])
            ? form["Specialty"].ToString()
            : "General Practice",
                        NationalId = nationalId,
                        LicenseNumber = form["LicenseNumber"].ToString() ?? string.Empty,
                        OrganizationId = org.OrganizationId
                    };

                    _db.Doctors.Add(doctor);
                    _db.SaveChanges();

                    HttpContext.Session.SetInt32("DoctorId", doctor.PractitionerId);
                    HttpContext.Session.SetString("Role", "Doctor");
                    HttpContext.Session.SetString("DoctorName", doctor.FullName);
                    return RedirectToAction("Dashboard", "Doctor");
                }
                catch (Exception ex)
                {
                    var msg = ex.InnerException?.InnerException?.Message
                           ?? ex.InnerException?.Message
                           ?? ex.Message;
                    ViewBag.Error = "Error: " + msg;
                    return View();
                }
            }
            // ===== PHARMACY =====
            if (role == "Pharmacy")
            {
                var pharmacist = new Pharmacist
                {
                    Name = fullName,
                    Phone = form["Phone"],
                    Email = form["Email"],
                    NationalId = nationalId,
                    LicenseNumber = form["LicenseNumber"],
                    Password = password,
                  
                };
                _db.Pharmacists.Add(pharmacist);
                _db.SaveChanges();
                HttpContext.Session.SetInt32("PharmacistId", pharmacist.Id);
                HttpContext.Session.SetString("Role", "Pharmacy");
                return RedirectToAction("Dashboard", "Pharmacy");
            }
            // LAB 
            if (role == "Lab")
            {
                var labTech = new LabTechnician
                {
                    Name = fullName,
                    Phone = form["Phone"],
                    Email = form["Email"],
                    NationalId = nationalId,
                    LicenseNumber = form["LicenseNumber"],
                    Password = password,
                    Speciality = !string.IsNullOrEmpty(form["Specialty"]) ? form["Specialty"].ToString() : "General Lab"
                };
                _db.LabTechnicians.Add(labTech);
                _db.SaveChanges();
                HttpContext.Session.SetInt32("LabTechId", labTech.Id);
                HttpContext.Session.SetString("Role", "Lab");
                return RedirectToAction("Dashboard", "Lab");
            }
            // ===== RADIOLOGY =====
            if (role == "Radiology")
            {
                var rad = new Radiologist
                {
                    FullName = fullName ?? string.Empty,
                    NationalId = nationalId ?? string.Empty,
                    Phone = form["Phone"].ToString() ?? string.Empty,
                    Email = form["Email"].ToString() ?? string.Empty,
                    Specialty = !string.IsNullOrEmpty(form["Specialty"]) ? form["Specialty"].ToString() : "General Radiology",
                    PasswordHash = password ?? string.Empty,
                    Status = "Active",
                    CreatedAt = DateTime.Now
                };
                _db.Radiologists.Add(rad);
                _db.SaveChanges();

                HttpContext.Session.SetInt32("RadiologistId", rad.Id);
                HttpContext.Session.SetString("Role", "Radiology");
                return RedirectToAction("Dashboard", "Radiology");
            }

            ViewBag.Error = "Something went wrong. Please try again.";
            return View();
        }

        // ===== LOGOUT =====
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}