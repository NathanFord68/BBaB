using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Profile
{
    public class UpdatePasswordModel
    {
        [Required(ErrorMessage = "This is a required field")]
        [Display(Name ="Old Password")]
        public string _oldPassword { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "New Password")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _newPassword { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Confirm New Password")]
        [Compare("_newPassword", ErrorMessage = "Your passwords did not match.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _confirmNewPassword { get; set; }


        public UpdatePasswordModel()
        {
            this._oldPassword = "";
            this._newPassword = "";
            this._confirmNewPassword = "";
        }

        public UpdatePasswordModel(string oldPassword, string newPassword, string confirmNewPassword)
        {
            this._oldPassword = oldPassword;
            this._newPassword = newPassword;
            this._confirmNewPassword = confirmNewPassword;
        }

        public PrincipalModel toPrincipal(PrincipalModel principal)
        {
            principal._credentials._password = this._newPassword;
            return principal;
        }
    }
}