using System.Net.Mail;
using System.Net;

using Microsoft.AspNetCore.Identity.UI.Services;

namespace Championship_Control_System.Utitlies
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("gamaleid820@gmail.com", "mxcw qxjb xcgp eyuw")
            };

            return client.SendMailAsync(
            new MailMessage(from: "gamaleid820@gmail.com",
                            to: email,
                            subject,
                            htmlMessage
                            )
            {
                IsBodyHtml = true
            });
        }
    }
}