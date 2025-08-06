using System.Net.Mail;
using System.Net;

namespace KhielsSkincare.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // Bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ft.g061004@gmail.com", "jfvvdbumlqinkzvt") // Nên sử dụng bảo mật cho thông tin nhạy cảm
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("ft.g061004@gmail.com"),
                Subject = subject,
                Body = message, // Nội dung HTML
                IsBodyHtml = true // Cho phép định dạng HTML
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
