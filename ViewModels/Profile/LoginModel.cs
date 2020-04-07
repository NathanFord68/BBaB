using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Profile

{
    public class LoginModel
    {
        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _email { get; set; }

        [Required(ErrorMessage = "This is a required field.")]
        [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Size must be between 5 and 50 characters.")]
        public string _password { get; set; }

        public LoginModel()
        {

        }
        public LoginModel(String email, String password)
        {
            this._email = email;
            this._password = password;
        }

        public PrincipalModel toPrincipal()
        {
            PrincipalModel principal = new PrincipalModel();
            principal._credentials = new CredentialModel(this._email, this._password);
            return principal;
        }

    }
}