using AppZeroAPI.Interfaces;
using AppZeroAPI.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace AppZeroAPI.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailConfiguration emailsettings;
        private static readonly string ConfirmEmailUrl = ConfigurationManager.AppSettings["ConfirmEmailUrl"];
        private static readonly string ConfirmChangeEmailUrl = ConfigurationManager.AppSettings["ConfirmChangeEmailUrl"];
        public EmailService(IConfiguration configuration)
        {
            this.emailsettings = configuration.GetSection(nameof(EmailConfiguration)).Get<EmailConfiguration>();
        }

        public void Send(string email, string subject, string body)
        {

            var smtpClient = GetSmtpClient();
            var mail = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                From = new MailAddress(emailsettings.Username, "ThreeFatties")
            };
            mail.To.Add(new MailAddress(email));
            smtpClient.Send(mail);
        }
        public void SendVerifyToEmail(string email, string userId, string baseUrl)
        {
            var smtpClient = GetSmtpClient();
            var mail = GetRegistrationMailMessage(email, userId, baseUrl);
            smtpClient.Send(mail);
        }

        public void SendLockoutNotification(string email, string login)
        {
            var smtpClient = GetSmtpClient();
            var mail = GetLockoutMailMessage(email, login);
            smtpClient.Send(mail);
        }

        public void SendChangeEmail(string userId, string newEmail, string baseUrl)
        {
            var smtpClient = GetSmtpClient();
            var mail = GetChangeEmailMailMessage(userId, newEmail, baseUrl);
            smtpClient.Send(mail);
        }

        public void SendSuperSecretCode(string email, string code)
        {
            var smtpClient = GetSmtpClient();
            var mail = GetSuperSecretMessage(email, code);
            smtpClient.Send(mail);
        }

        private SmtpClient GetSmtpClient()
        {
            return new SmtpClient(emailsettings.SmtpServer, emailsettings.Port)
            {
                Credentials = new NetworkCredential(emailsettings.Username, emailsettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
        }

        private MailMessage GetRegistrationMailMessage(string email, string userId, string baseUrl)
        {
            var mail = new MailMessage
            {
                Subject = "Завершение регистрации",
                Body = GenerateRegistrationMailBody(email, userId, baseUrl),
                IsBodyHtml = true,
                From = new MailAddress(emailsettings.Username, "ThreeFatties")
            };
            mail.To.Add(new MailAddress(email));
            return mail;
        }

        private string GenerateRegistrationMailBody(string email, string userId, string baseUrl)
        {
            var urlString = String.Format("{0}/{1}?token={2}&email={3}", baseUrl, ConfirmEmailUrl, userId, email);
            return String.Format("Для завершения регистрации перейдите по ссылке: <a href={0}>click</a>", urlString);
        }

        private MailMessage GetChangeEmailMailMessage(string userId, string email, string baseUrl)
        {
            var mail = new MailMessage
            {
                Subject = "Подтверждение изменения email",
                Body = GenerateChangeEmailMailBody(email, userId, baseUrl),
                IsBodyHtml = true,
                From = new MailAddress(emailsettings.Username, "ThreeFatties")
            };
            mail.To.Add(new MailAddress(email));
            return mail;
        }

        private string GenerateChangeEmailMailBody(string email, string userId, string baseUrl)
        {
            var urlString = String.Format("{0}/{1}?token={2}&email={3}", baseUrl, ConfirmChangeEmailUrl, userId, email);
            return String.Format("Для подтверждения смены email перейдите по ссылке: <a href={0}>click</a>", urlString);
        }

        private MailMessage GetLockoutMailMessage(string email, string login)
        {
            var mail = new MailMessage
            {
                Subject = "Сообщение о блокировке",
                Body = GenerateLockoutMailBody(login),
                IsBodyHtml = true,
                From = new MailAddress(emailsettings.Username, "ThreeFatties")
            };
            mail.To.Add(new MailAddress(email));
            return mail;
        }

        private string GenerateLockoutMailBody(string login)
        {
            return String.Format("Количество неудачных попыток ввода пароля пользователя с именем {0} превышено. Попробуйте снова через 15 минут.", login);
        }

        private MailMessage GetSuperSecretMessage(string email, string code)
        {
            var mail = new MailMessage
            {
                Subject = "Суперсекретный код для регистрации",
                Body = GetSuperSecretMessageBody(code),
                IsBodyHtml = true,
                From = new MailAddress(emailsettings.Username, "ThreeFatties")
            };
            mail.To.Add(new MailAddress(email));
            return mail;
        }

        private string GetSuperSecretMessageBody(string code)
        {
            return String.Format("Ваш суперсекретный код для регистрации и просмотра профиля: {0}.", code);
        }
    }
}
