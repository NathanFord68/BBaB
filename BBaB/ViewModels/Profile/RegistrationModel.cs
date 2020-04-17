using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Profile
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _fullName { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "User Name")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _username { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _password { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Confirm Password")]
        [Compare("_password", ErrorMessage = "Your passwords did not match.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _confirmPassword { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "This must be an email.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _email { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Phone Number")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _phoneNumber { get; set; }

        public RegistrationModel()
        {

        }

        public RegistrationModel(String fullName, String username, String password, String email, string phoneNumber)
        {
            this._fullName = fullName;
            this._username = username;
            this._password = password;
            this._email = email;
            this._phoneNumber = phoneNumber;
        }

        public PrincipalModel ToPrincipal()
        {
            PrincipalModel principal = new PrincipalModel();
            principal._fullName = this._fullName;
            principal._userName = this._username;
            principal._credentials._password = this._password;
            principal._credentials._email = this._email;
            principal._phoneNumber = this._phoneNumber;

            return principal;
        }
    }
}