using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utility
{
    public class EmailSender : IEmailSender
    {
        private  readonly MimeMessage emailToSend;
        public EmailSender()
        {
            emailToSend = new MimeMessage();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            emailToSend.From.Add(MailboxAddress.Parse(SD.EmailSendFrom));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage};

            // send
            using (var emailClient = new SmtpClient())
            {
                await emailClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await emailClient.AuthenticateAsync("lethanhtong.programing@gmail.com", "fqyhatpchsoxzoqn");
                await emailClient.SendAsync(emailToSend);
                await emailClient.DisconnectAsync(true); 
            }
        }
    }
}
