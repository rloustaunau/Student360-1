using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Resources.Infrastructure.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Messaging
{
    public class EmailMessagingProvider : IMessagingProvider
    {
        private readonly string _defaultFromAddress;
        private readonly string _defaultFromDisplayName;
        private readonly string _mailServer;
        private readonly string _mailPort;
        private readonly string _mailUser;
        private readonly string _mailPassword;
        private readonly IConfiguration _config;

        public EmailMessagingProvider(IConfiguration config)
        {
            _config = config;
            // Initialize with parameters set in the Web.Config
            _defaultFromAddress = _config["Messaging:Email:DefaultFromEmail"];
            _defaultFromDisplayName = _config["Messaging:Email:DefaultFromDisplayName"];
            _mailServer = _config["Messaging:Email:Server"];
            _mailPort = _config["Messaging:Email:Port"];
            _mailUser = _config["Messaging:Email:User"];
            _mailPassword = _config["Messaging:Email:Pass"];
        }

        public async Task SendMessageAsync(string to, string[] cc, string[] bcc, string subject, string body)
        {
            await SendMessageAsync(new string[] { to }, cc, bcc, subject, body);
        }

        public async Task SendMessageAsync(string[] to, string[] cc, string[] bcc, string subject, string body)
        {
            var defaultFrom = new MailAddress(_defaultFromAddress, _defaultFromDisplayName);
            await SendEmailAsync(defaultFrom, to, cc, bcc, null, subject, body);
        }

        public async Task SendMessageAsync(string from, string[] to, string[] cc, string[] bcc, string subject, string body)
        {
            var fromAddress = new MailAddress(from);
            await SendEmailAsync(fromAddress, to, cc, bcc, null, subject, body);
        }

        public async Task SendMessageAsync(string[] to, string[] cc, string[] bcc, string[] replyTo, string subject, string body)
        {
            var defaultFrom = new MailAddress(_defaultFromAddress, _defaultFromDisplayName);
            await SendEmailAsync(defaultFrom, to, cc, bcc, replyTo, subject, body);
        }

        private async Task SendEmailAsync(MailAddress from, string[] to, string[] cc, string[] bcc, string[] replyTo, string subject, string body)
        {
            // Create mail credentials and client
            //var credentials = new NetworkCredential(_mailUser, _mailPassword);
            var smtpClient = new SmtpClient(_mailServer, Convert.ToInt32(_mailPort));

            // Create mail message
            var mailMessage = new MailMessage()
            {
                From = from,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // Allow for multiple To, CC and BCC
            if (!to.IsNullOrEmpty())
                to.ToList().ForEach(x => mailMessage.To.Add(new MailAddress(x)));

            if (!cc.IsNullOrEmpty())
                cc.ToList().ForEach(x => mailMessage.CC.Add(new MailAddress(x)));

            if (!bcc.IsNullOrEmpty())
                bcc.ToList().ForEach(x => mailMessage.Bcc.Add(new MailAddress(x)));

            if (!replyTo.IsNullOrEmpty())
                replyTo.ToList().ForEach(x => mailMessage.ReplyToList.Add(new MailAddress(x)));

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
