using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using CWSStart.Web.Models;
using Umbraco.Web;

namespace CWSStart.Web.CWSExtensions
{
    public static class EmailHelper
    {
        private const string SMTPServer = ""; //"smtp.mandrillapp.com";
        private const string SMTPUser = ""; //"warren@creativewebspecialist.co.uk";
        private const string SMTPPassword = ""; //"h4GMK-gX9CB7KXjUePMNaA";
        private const int SMTPPort = 25;

        public static SmtpClient GetSmtpClient()
        {

            //Get default SMTP server settings from our hompage node
            var homepage = UmbracoContext.Current.ContentCache.GetAtRoot().SingleOrDefault(x => x.DocumentTypeAlias == "CWS-Home");

            //Get values from homenode, with fallback to Mandrill constant's above
            var server = homepage.GetPropertyValue("smtpServer", SMTPServer).ToString();
            var user = homepage.GetPropertyValue("smtpUsername", SMTPUser).ToString();
            var pass = homepage.GetPropertyValue("smtpPassword", SMTPPassword).ToString();
            int port;
            bool useSsl = Convert.ToBoolean(homepage.GetPropertyValue("useSsl").ToString());

            //Do a null check just in case homepage node values are empty (fallback to Constants)
            server = String.IsNullOrEmpty(server) ? SMTPServer : server;
            user = String.IsNullOrEmpty(user) ? SMTPUser : user;
            pass = String.IsNullOrEmpty(pass) ? SMTPPassword : pass;
            if (!int.TryParse(homepage.GetPropertyValue("smtpPort", SMTPPort).ToString(), out port))
            {
                port = SMTPPort;
            }

            //Create new SmtpClient
            var smtp = new SmtpClient();
            smtp.Host = server;
            smtp.EnableSsl = useSsl;
            smtp.Credentials = new NetworkCredential(user, pass);
            smtp.Port = port;

            //Return the SMTP object
            return smtp;
        }


        public static void SendContactEmail(ContactFormViewModel model, string emailTo, string emailSubject)
        {
            //Create email address with friendly display names
            MailAddress emailAddressFrom = new MailAddress(model.Email, model.Name);
            MailAddress emailAddressTo = new MailAddress(emailTo, "Contact Form");

            //Generate an email message object to send
            MailMessage email = new MailMessage(emailAddressFrom, emailAddressTo);
            email.Subject = emailSubject;
            email.Body = model.Message;

            try
            {
                //Connect to SMTP using MailChimp transactional email service Mandrill
                //This uses the values on the homenode OR fallback to test details above
                SmtpClient smtp = GetSmtpClient();

                //Try & send the email with the SMTP settings
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                //Throw an exception if there is a problem sending the email
                throw ex;
            }

        }

        public static void SendResetPasswordEmail(string memberEmail, string emailFrom, string emailSubject, string resetGUID)
        {
            //Reset link
            string baseURL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
            var resetURL = baseURL + "/reset-password?resetGUID=" + resetGUID;

            var message = string.Format(
                                "<h3>Reset Your Password</h3>" +
                                "<p>If you have not requested to reset your password please ignore and delete this email.</p>" +
                                "<p><a href='{0}'>Reset your password</a></p>",
                                resetURL);

            //Create email message to send
            var email = new MailMessage(emailFrom, memberEmail);
            email.Subject = emailSubject;
            email.IsBodyHtml = true;
            email.Body = message;

            try
            {
                //Connect to SMTP using MailChimp transactional email service Mandrill
                //Connect to SMTP using MailChimp transactional email service Mandrill
                //This uses the values on the homenode OR fallback to test details above
                SmtpClient smtp = GetSmtpClient();

                //Try & send the email with the SMTP settings
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                //Throw an exception if there is a problem sending the email
                throw ex;
            }

        }

        public static void SendForgottenPasswordEmail(string memberEmail, string memberPassword, string emailFrom, string emailSubject, string verifyGUID)
        {

            var message = string.Format(
                               "<h3>Your Password</h3>" +
                               "<p>Here is your forgotten password:</p>"+
                               "<p><b>" + memberPassword + "</b></p>"+
                               "<p>For security purposes please delete and remove this email from your trash as soon as possible.</p>");

            MailMessage email = new MailMessage(emailFrom, memberEmail);
            email.Subject = emailSubject;
            email.IsBodyHtml = true;
            email.Body = message;

            try
            {
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
            }
            catch (Exception ex) {
                
                throw ex;
            }
        }

        public static void SendVerifyEmail(string memberEmail, string emailFrom, string emailSubject, string verifyGUID)
        {
            //Verify link
            string baseURL = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty);
            var verifyURL = baseURL + "/verify-email?verifyGUID=" + verifyGUID;

            var message = string.Format(
                                "<h3>Verify Your Email</h3>" +
                                "<p>Click here to verify your email address and activate your account.</p>" +
                                "<p><a href='{0}'>Verify your email & activate your account</a></p>",
                                verifyURL);

            //Create email message to send
            var email = new MailMessage(emailFrom, memberEmail);
            email.Subject = emailSubject;
            email.IsBodyHtml = true;
            email.Body = message;

            try
            {
                //Connect to SMTP using MailChimp transactional email service Mandrill
                //This uses the values on the homenode OR fallback to test details above
                SmtpClient smtp = GetSmtpClient();

                //Try & send the email with the SMTP settings
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                //Throw an exception if there is a problem sending the email
                throw ex;
            }
        }

        public static void SendSignUpEmailToAdmin(string memberEmail, string emailFrom, string emailSubject, string verifyGUID)
        {
            MailMessage email = new MailMessage();

            try
            {   
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email); //Try & send the email with the SMTP settings
            }
            catch (Exception ex)
            {
                throw ex; //Throw an exception if there is a problem sending the email
            }
        }
    }
}
