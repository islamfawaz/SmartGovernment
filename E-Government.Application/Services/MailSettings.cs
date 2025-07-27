using E_Government.Application.DTO;
using E_Government.Application.Helper;
using E_Government.Application.ServiceContracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services
{
    internal class  MailSettings : IMailSettings
    {
        private readonly IOptions<EmailSettings> _options;

        public MailSettings(IOptions<EmailSettings> options)
        {
            _options = options;
        }
        public void SendEmail(Email email)
        {
            var mail = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_options.Value.Email),
                Subject = email.Subject,
            };
            mail.To.Add(MailboxAddress.Parse(email.To));    
            mail.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));
            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);  
            smtp.Authenticate(_options.Value.Email, _options.Value.Password);

            smtp.Send(mail);

            smtp.Disconnect(true);

        }
    }
}
