using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace HealthMSR.BLL.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendResetEmail(string toEmail, string resetLink)
        {
            var smtp = new SmtpClient(_config["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_config["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:SenderPassword"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"], "HealthMSR"),
                Subject = "Reset Your Password — HealthMSR",
                IsBodyHtml = true,
                Body = $@"
                    <div style='font-family:sans-serif; max-width:500px; margin:auto;'>
                        <h2 style='color:#0369a1;'>HealthMSR Password Reset</h2>
                        <p>Click the button below to reset your password:</p>
                        <a href='{resetLink}'
                           style='display:inline-block; padding:14px 28px; background:#0369a1; color:white; border-radius:12px; text-decoration:none; font-weight:700;'>
                            Reset Password
                        </a>
                        <p style='color:#64748b; font-size:0.85rem; margin-top:20px;'>
                            This link expires in 30 minutes.
                        </p>
                    </div>"
            };
            mail.To.Add(toEmail);
            await smtp.SendMailAsync(mail);
        }
    }
}