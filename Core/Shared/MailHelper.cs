using System.Net.Mail;
using System.Net;
using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Reflection;
using AppZeroAPI.Shared.Constants;
using AppZeroAPI.Shared.Enums;
using System.Collections.Generic;

namespace AppZeroAPI.Shared
{

    public class SMTPSettingsModel
    {
        public SMTPSettingsModel() { }
        public string Host
        { get; set; }
        public string Sender
        { get; set; }
        public string Password
        { get; set; }
        public string UserName
        { get; set; }
        public string ActivationMailSubject
        { get; set; }
        public string ConfirmationMailsubject
        { get; set; }
        public bool EnableSSl
        { get; set; }
        public int Port
        { get; set; }
        public string ActivationURL
        { get; set; }
        public string Invitation
        { get; set; }
        public string SystemAdminEmail
        { get; set; }
        public string FormURL
        { get; set; }
        public string SuperAdminModules { get; set; }
        public string SuperAdminEmail { get; set; }

        public string DefaultPlan { get; set; }
        public string SkipDefaultPlanError { get; set; }
        public string BiometricPathURL
        { get; set; }
        public string TempBiometricPathURL
        { get; set; }

        public string ResetPasswordURL
        { get; set; }


    }

    public class MailHelper
    {
        public MailHelper(string receiveremail, string link, string receiverName, string templateType, string templateBody, string subject, string workSpaceName, SMTPSettingsModel sMTPSettingsModel, string message, string notificationEmailMessage, string notificationEmailFromValue, string levelName, string entryFilledByUser,string entryDetails)
        {
            templateBody = UpdateTemplateBody(link, receiverName, templateType, templateBody, workSpaceName, message, notificationEmailMessage, levelName, entryFilledByUser, "", "", "", "", receiveremail,"",entryDetails);
            SendHtmlFormattedEmail(templateBody, receiveremail, subject, sMTPSettingsModel, workSpaceName, templateType, notificationEmailFromValue, "", "");
        }

        public MailHelper(string receiveremail, string link, string receiverName, string templateType, string templateBody, string subject, string workSpaceName, SMTPSettingsModel sMTPSettingsModel, string attachmentPath, string attachmentName, string amount, string chargeId)
        {
            templateBody = UpdateTemplateBody(link, receiverName, templateType, templateBody, workSpaceName, "", "", "", "", "", amount, chargeId, "", "","", "");
            SendHtmlFormattedEmail(templateBody, receiveremail, subject, sMTPSettingsModel, workSpaceName, templateType, "", attachmentPath, attachmentName);
        }

        public MailHelper(string receiveremail, string link, string receiverName, string templateType, string templateBody, string subject, string workSpaceName, SMTPSettingsModel sMTPSettingsModel, string formName)
        {
            templateBody = UpdateTemplateBody(link, receiverName, templateType, templateBody, workSpaceName, "", "", "", "", formName, "", "", "",receiveremail, "", "");
            SendHtmlFormattedEmail(templateBody, receiveremail, subject, sMTPSettingsModel, workSpaceName, templateType, "", "", "");
        }

        public MailHelper(string receiveremail, string link, string receiverName, string templateType, string templateBody, string subject, string workSpaceName, SMTPSettingsModel sMTPSettingsModel, string formName, string oldEmail)
        {
            templateBody = UpdateTemplateBody(link, receiverName, templateType, templateBody, workSpaceName, "", "", "", "", formName, "", "", oldEmail, receiveremail,"", "");
            SendHtmlFormattedEmail(templateBody, receiveremail + ";" + oldEmail, subject, sMTPSettingsModel, workSpaceName, templateType, "", "", "");
        }

        public MailHelper(string senderEmail, string senderName, string message, string templateType, string templateBody, string subject, SMTPSettingsModel sMTPSettingsModel, string userEmail)
        {
            templateBody = UpdateTemplateBody("", senderName, templateType, templateBody, "", message, "", "", "", "", "", "", userEmail, "","", "");
            SendHtmlFormattedEmail(templateBody, senderEmail, subject, sMTPSettingsModel, "",templateType, "", "", "");
        }

        public MailHelper(string receiveremail, string receiverName, string templateType, string templateBody, string subject, string workSpaceName, SMTPSettingsModel sMTPSettingsModel,string userEmail,string planName)
        {
            templateBody = UpdateTemplateBody("", receiverName, templateType, templateBody, workSpaceName, "", "","","", "", "", "", userEmail, "",planName,"");
            SendHtmlFormattedEmail(templateBody, receiveremail, subject, sMTPSettingsModel,"", templateType, "", "", "");
        }
        private static string UpdateTemplateBody(string link, string receiverName, string templateType, string templateBody, string workSpaceName, string message, string notificationEmailMessage, string levelName, string entryFilledByUser, string formName, string amount, string chargeId, string oldEmail, string receiveremail,string planName,string entryDetails)
        {
            if (templateType == TemplateTypes.ActivationEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{ActivationLink}}", link);
            }
            else if (templateType == TemplateTypes.ContactEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{Email}}", oldEmail);
                templateBody = templateBody.Replace("{{message}}", message);
                templateBody = templateBody.Replace("{{CurrentDate}}", DateTime.UtcNow.ToShortDateString());
            }
            else if (templateType == TemplateTypes.ResetPasswordURLEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{ResetLink}}", link);
            }
            else if (templateType == TemplateTypes.NotificationEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{ActivationLink}}", link);
                templateBody = templateBody.Replace("{{LevelName}}", levelName);
                templateBody = templateBody.Replace("{{FilledByUserName}}", entryFilledByUser);
            }
            else if (templateType == TemplateTypes.ConfirmationEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
            }
            else if (templateType == TemplateTypes.ShareRefferalLink.ToString())
            {
                templateBody = templateBody.Replace("{{WorkspaceName}}", workSpaceName);
                templateBody = templateBody.Replace("{{UserName}}", receiveremail);
                templateBody = templateBody.Replace("{{Message}}", message);
                templateBody = templateBody.Replace("{{RefferalLink}}", link);
            }
            else if (templateType == TemplateTypes.SendEmailOnsubmit.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{Message}}", notificationEmailMessage);
                templateBody = templateBody.Replace("{{EntryDetails}}", entryDetails);
                templateBody = templateBody.Replace("{{EntryLink}}", link);
            }
            else if (templateType == TemplateTypes.UserAddedToWorkflow.ToString() || templateType == TemplateTypes.UserDeletedFromWorkFlow.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiveremail);
                templateBody = templateBody.Replace("{{WorkFlowName}}", formName);
            }
            else if (templateType == TemplateTypes.PaymentConfirmationEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{Amount}}", amount);
                templateBody = templateBody.Replace("{{ChargeId}}", chargeId);
                templateBody = templateBody.Replace("{{Date}}", DateTime.UtcNow.ToString());
            }
            else if (templateType == TemplateTypes.FormPaymentConfirmationEmail.ToString())
            {
                templateBody = templateBody.Replace("{{Amount}}", amount);
                templateBody = templateBody.Replace("{{ChargeId}}", chargeId);
                templateBody = templateBody.Replace("{{Date}}", DateTime.UtcNow.ToString());
            }
            else if (templateType == TemplateTypes.ChangeUserEmail.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", workSpaceName);
                templateBody = templateBody.Replace("{{Email}}", receiveremail);
                templateBody = templateBody.Replace("{{OldEmail}}", oldEmail);
            }
            else if (templateType == TemplateTypes.BlazeFormDeleteOrganization.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{OrganizationName}}", workSpaceName);
                templateBody = templateBody.Replace("{{CurrentDate}}", DateTime.UtcNow.ToShortDateString());
            }
            else if (templateType == TemplateTypes.BlazeFormActivationOrganization.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{OrganizationName}}", workSpaceName);
                templateBody = templateBody.Replace("{{CurrentDate}}", DateTime.UtcNow.ToShortDateString());
            }
            else if (templateType == TemplateTypes.BlazeFormDeActivationOrganization.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{OrganizationName}}", workSpaceName);
                templateBody = templateBody.Replace("{{CurrentDate}}", DateTime.UtcNow.ToShortDateString());
            }
            
            else if (templateType == TemplateTypes.EmailToSuperAdmin.ToString())
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{OrganizationName}}", workSpaceName);
                templateBody = templateBody.Replace("{{UserEmail}}", oldEmail);
                templateBody = templateBody.Replace("{{PlanName}}", planName);
                templateBody = templateBody.Replace("{{CurrentDate}}", DateTime.UtcNow.ToShortDateString());
            }
            else
            {
                templateBody = templateBody.Replace("{{UserName}}", receiverName);
                templateBody = templateBody.Replace("{{InviteLink}}", link);
                templateBody = templateBody.Replace("{{WorkspaceName}}", workSpaceName);
            }

            return templateBody;
        }

        private void SendHtmlFormattedEmail(string templateBody, string receiveremail, string subject, SMTPSettingsModel sMTPSettingsModel, string workSpaceName, string templateType, string notificationEmailFromValue, string attachmentPath, string attachmentName)
        {
            using (MailMessage mailMessage = new MailMessage())
            {

                if (templateType == TemplateTypes.ShareRefferalLink.ToString())
                {
                    mailMessage.From = new MailAddress(sMTPSettingsModel.Sender, "Blazeforms! Referred a Blazeforms Link");
                }
                else if (templateType == TemplateTypes.SendEmailOnsubmit.ToString())
                {
                    mailMessage.From = new MailAddress(sMTPSettingsModel.Sender, notificationEmailFromValue);
                }  
                else
                {
                    mailMessage.From = new MailAddress(sMTPSettingsModel.Sender);
                }

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(@attachmentPath);
                    attachment.Name = attachmentName;
                    mailMessage.Attachments.Add(attachment);
                }
                
                mailMessage.Subject = subject;

                mailMessage.Body = templateBody;

                mailMessage.IsBodyHtml = true;

                foreach (var address in receiveremail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(address);
                }

                SmtpClient smtp = new SmtpClient();

                smtp.Host = sMTPSettingsModel.Host;

                smtp.EnableSsl = sMTPSettingsModel.EnableSSl;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = sMTPSettingsModel.UserName;

                NetworkCred.Password = sMTPSettingsModel.Password;

                smtp.UseDefaultCredentials = false;

                smtp.Credentials = NetworkCred;

                smtp.Port = sMTPSettingsModel.Port;
                smtp.Timeout = 30000000;

                try
                {
                    //ServiceReference1.Service1Client clientData = new ServiceReference1.Service1Client();
                    //var response = (clientData.SendEmailAsync(receiveremail, templateBody, subject)).Result;
                    smtp.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        
        #region commented Code
        //public string GenerateMailBody(string linkToActivate, string userName, string title, string message)
        //{
        //    if (linkToActivate != "")
        //    {
        //        StringBuilder str = new StringBuilder();
        //        str.AppendLine("Hi {UserName},");
        //        str.AppendLine("This is an Activation request email. please click on the below link to activate your blaze form account.");
        //        str.AppendLine("{ActivationLink}");
        //        str.AppendLine("With Regards");
        //        str.AppendLine("Team Blaze Forms");
        //        string body = "";
        //        body = str.ToString();
        //        body = body.Replace("{UserName}", userName); //replacing the required things  
        //        body = body.Replace("{ActivationLink}", linkToActivate);
        //        return body;
        //    }
        //    else
        //    {
        //        StringBuilder str = new StringBuilder();
        //        str.AppendLine("Hi {UserName},");
        //        str.AppendLine("This is an confirmation email. Your Account is successfully activated.");
        //        str.AppendLine("With Regards");
        //        str.AppendLine("Team Blaze Forms");
        //        string body = "";
        //        body = str.ToString();
        //        body = body.Replace("{UserName}", userName); //replacing the required things  
        //        return body;
        //    }
        //}

        #endregion

    }
}
