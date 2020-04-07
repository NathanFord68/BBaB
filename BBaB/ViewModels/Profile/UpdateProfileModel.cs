using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Profile
{
    public class UpdateProfileModel
    {
        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _fullName { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "User Name")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _username { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "This must be an email.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _email { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Phone Number")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _phoneNumber { get; set; }

        public UpdateProfileModel()
        {
            this._fullName = "";
            this._email = "";
            this._phoneNumber = "";
            this._username = "";
        }

        public UpdateProfileModel(string fullName, string email, string phoneNumber, string userName)
        {
            this._fullName = fullName;
            this._email = email;
            this._phoneNumber = phoneNumber;
            this._username = userName;
        }

        public PrincipalModel toPrincipal(PrincipalModel principal)
        {
            principal._fullName = this._fullName;
            principal._credentials._email = this._email;
            principal._phoneNumber = this._phoneNumber;
            principal._userName = this._username;
            return principal;
        }
    }
}