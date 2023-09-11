using System;
using System.Net;
using System.Net.Mail;
using AspNetCoreIdentityApp.Core.OptionsModels;
using Microsoft.Extensions.Options;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string toEmail)
        {
            var smptClient = new SmtpClient();
            smptClient.Host = _emailSettings.Host;
            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.Port = 587;
            smptClient.UseDefaultCredentials = false;
            smptClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.To.Add(toEmail);

            mailMessage.Subject = "Localhost | Sifre Sifirlama Linki";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = @$"
                            <h4> Şifrenizi yenilemek için aşağıdaki linke tıklayınız.</h4>
                            <p><a href='{resetPasswordEmailLink}'> Şifre Yenileme Link</a></p>";

            await smptClient.SendMailAsync(mailMessage);
        }
    }
}

