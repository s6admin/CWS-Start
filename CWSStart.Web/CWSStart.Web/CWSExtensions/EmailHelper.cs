﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using CWSStart.Web.Models;
using Umbraco.Web;
using Umbraco.Core.Logging;
using System.Net.Sockets;

namespace CWSStart.Web.CWSExtensions
{
    public static class EmailHelper
    {
        private const string SMTPServer = "";
        private const string SMTPUser = "";
        private const string SMTPPassword = "";
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

        public static void SendQuestionsEmail(int memberId, string memberDisplayName, string memberEmail, string adminEmail, string message)
        {
            //Create email address with friendly display names
            MailAddress emailAddressFrom = new MailAddress(memberEmail, memberDisplayName);
            MailAddress emailAddressTo = new MailAddress(adminEmail, "Questions Form");

            //Generate an email message object to send
            MailMessage email = new MailMessage(emailAddressFrom, emailAddressTo);
            email.Subject = "Questions Form"; 
            email.Body = message;

            try
            {   
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "Questions Form Submission - from:" + memberEmail + " (" + memberId + "), message: " + message);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmailHelper), "Questions Form Submission Error ", ex);
                throw ex;                
            }
        }

        public static void SendAccountActivationEmail(string memberEmail, string emailFrom, string emailSubject, string emailMessage)
        {
         
            MailMessage email = new MailMessage(emailFrom, memberEmail);
            email.Subject = emailSubject;
            email.IsBodyHtml = true;
            email.Body = emailMessage;

            try
            {
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "Account Activation Email Sent - for:" + memberEmail);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmailHelper), "Account Activation Email Send Error ", ex);
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
                SmtpClient smtp = GetSmtpClient();                
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "Reset Password Email Sent - for:" + memberEmail);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmailHelper), "Reset Password Email Send Error ", ex);
                throw ex;
            }

        }

        public static void SendForgottenPasswordEmail(string memberEmail, string memberPassword, string emailFrom)
        {

            var message = string.Format(
                               "<h3>Your Password</h3>" +
                               "<p>Here is your forgotten password:</p>"+
                               "<p><b>" + memberPassword + "</b></p>"+
                               "<p>For security purposes please delete and remove this email from your trash as soon as possible.</p>");

            MailMessage email = new MailMessage(emailFrom, memberEmail);
            email.Subject = "Your Forgotten Password";
            email.IsBodyHtml = true;
            email.Body = message;

            try
            {
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "Forgotten Password Email Sent - for:" + memberEmail);
            }
            catch (Exception ex) {
                LogHelper.Error(typeof(EmailHelper), "Forgotten Password Email Send Error ", ex);
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
        
        public static void SendSignUpEmailToAdmin(RegisterViewModel model, string adminEmail)
        {
                       
            string clientIPAddress = Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();                        
            string message =
                "<h3>Sign Up Request</h3>" +
                "<p></p>" +
                "<p> Member Type: " + model.MembershipType + "</p>" +
                "<p>Contact Name: " + model.Name + "</p>" +
                "<p>Company Name: " + model.CompanyName + "</p>" +
                "<p> Sold To Num: " + model.SoldToNum + "</p>" +
                "<p>       Email: " + model.EmailAddress + "</p>" +
                "<p>       Phone: " + model.Phone + "</p>" +
                //"<p>          IP: " + clientIPAddress +"</p>" +                
                "";
            MailMessage email = new MailMessage(model.EmailAddress, adminEmail);
            email.Subject = "Sign Up Request";
            email.IsBodyHtml = true;
            email.Body = message;

            try
            {   
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "Sign Up Email Sent To Admin - for:" + model.EmailAddress);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmailHelper), "Sign Up Email Send Error ", ex);
                throw ex; 
            }
        }

        /// <summary>
        /// UNUSED. Forgotten passwords are sent directly to the end user via email. Sends a forgotten password email reminder to the admin account from a user.
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="memberEmail"></param>
        /// <param name="adminEmail"></param>
        public static void SendForgottenPasswordEmailToAdmin(string memberName, string memberEmail, string adminEmail)
        {
            string message =
                "<h3>Forgotten Password</h3>" +
                "<p>" + memberName + " has forgotten their password.</p>" +
                "<p>Please send a password reminder to " + memberEmail + " from the Members dashboard.</p>";

            MailMessage email = new MailMessage(memberEmail, adminEmail);
            email.Subject = "Forgotten Password";
            email.IsBodyHtml = true;
            email.Body = message;
            try
            {
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendOrderMyBrewersEmailToAdmin(string memberName, string memberEmail, string adminEmail, string itemsTable, float totalPrice, int totalItems)
        {
            string message =
               "<h3>Order My Brewers</h3>" +
               "<p>" + memberName + " (" + memberEmail + ") has ordered new items.</p>" +
               "<p></p>" +
               itemsTable + 
               "<p></p>"+
               "<p>Total Items: " + totalItems.ToString() + "<br/>" +
               "<p>Total Price: $" + totalPrice.ToString() + "<br/>" +
               "";

            MailMessage email = new MailMessage(memberEmail, adminEmail);
            email.Subject = "New Brewers Order";
            email.IsBodyHtml = true;
            email.Body = message;
            try
            {
                SmtpClient smtp = GetSmtpClient();
                smtp.Send(email);
                LogHelper.Info(typeof(EmailHelper), "New Brewers Order - for: " + memberEmail);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(EmailHelper), "New Brewers Order Error ", ex);
                throw ex;
            }
        }
    }
}
