using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        private const string SendGridKey = "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";
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(SendGridKey, subject, htmlMessage, email);
        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("",""),
                Subject = subject,
                PlainTextContent=message,
                HtmlContent=message
            };

            msg.AddTo(new EmailAddress(email));

            return client.SendEmailAsync(msg);
        }
    }
}
