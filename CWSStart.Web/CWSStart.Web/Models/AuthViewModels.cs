using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CWSStart.Web.Models
{
    /// <summary>
    /// Login View Model
    /// </summary>
    public class LoginViewModel
    {
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// Register View Model
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please select your desired membership tier")]
        public string MembershipType { get; set; }
        
        [DisplayName("Company Name")]
        [Required(ErrorMessage = "Please enter your Company's name")]
        public string CompanyName { get; set; }

        [DisplayName("Sold To #")]
        [Required(ErrorMessage = "Please enter your Sold To Number")]
        public string SoldToNum { get; set; }
        
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } // S6: Contact Name

        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Remote("CheckEmailIsUsed", "AuthSurface", ErrorMessage = "The email address has already been registered")]
        public string EmailAddress { get; set; }

        [DisplayName("Phone")]
        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "You must agree to the Terms and Conditions")]        
        public bool AgreedToTerms { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string IPAddress { get; set; }

        /* S6: Password is being removed from the registration process because passwords are admin-controlled. */
        /*[DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your passwords do not match")]
        public string ConfirmPassword { get; set; }*/

        /* S6: Removed ProfileURL is not a neccessary field for Members */
        /*[Required]
        [Remote("CheckProfileURLAvailable", "ProfileSurface", ErrorMessage = "The profile URL is already in use")]
        [DisplayName("Profile URL")]
        public string ProfileURL { get; set; }*/

    }

    //Forgotten Password View Model
    public class ForgottenPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
    }


    //Reset Password View Model
    public class ResetPasswordViewModel
    {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}