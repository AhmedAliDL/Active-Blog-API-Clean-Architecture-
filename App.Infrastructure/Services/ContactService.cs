using App.Application.Common.Interfaces.Services;
using App.Application.Notifications.Dto;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace App.Infrastructure.Services
{
    public class ContactService(IConfiguration configuration, ILogger<ContactService> logger) : IContactService
    {
        private async Task<NotifyDto> SendInformationToServerAsync(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    logger.LogInformation("Send message to server operation started.");
                    await client.ConnectAsync(
                        configuration["SmtpSettings:SMTPServer"]!,
                        int.Parse(configuration["SmtpSettings:Port"]!),
                        false);

                    var userName = configuration["SmtpSettings:SmtpEmail"]!;
                    var emailPassword = configuration["SmtpSettings:Password"]!;

                    await client.AuthenticateAsync(userName, emailPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    logger.LogInformation("Send message to server operation completed.");
                }
                catch
                {
                    return new NotifyDto
                    {
                        Success = false,
                        Message = "Error happened while sending data to server."
                    };
                }
            }
            return new NotifyDto
            {
                Success = true,
                Message = "Message sent successfuly"
            }; ;
        }
        public async Task<NotifyDto> SendEmailServiceAsync(string toEmailAdd, string fromEmailAdd, string subject, string body, string fromName, string toName)
        {

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(fromName, fromEmailAdd));

            message.To.Add(new MailboxAddress(toName, toEmailAdd));


            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            return await SendInformationToServerAsync(message);
        }
        public async Task<NotifyDto> SendEmailToAdminServiceAsync(string toEmailAdd, string replaytoEmailAdd, string subject, string body, string fromName, string toName)
        {

            var message = new MimeMessage();
            message.From.Add(
            new MailboxAddress(
                "Active Blog",
            configuration["SmtpSettings:AdminEmail"]!));

            message.ReplyTo.Add(
                new MailboxAddress(
                    fromName,
                    replaytoEmailAdd));

            message.To.Add(
                new MailboxAddress(
                    toName,
                    toEmailAdd));


            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            return await SendInformationToServerAsync(message);
        }
    }

}