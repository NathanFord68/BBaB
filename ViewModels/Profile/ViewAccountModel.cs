using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Profile
{
    public class ViewAccountModel
    {
        public int _id { get; set; }

        [Display(Name = "Full Name")]
        public string _fullName { get; set; }

        [Display(Name = "User Name")]
        public string _userName { get; set; }

        [Display(Name = "Phone Number")]
        public string _phoneNumber { get; set; }

        [Display(Name = "Email")]
        public string _email { get; set; }


        public ViewAccountModel()
        {
            this._id = -1;
            this._fullName = "";
            this._userName = "";
            this._phoneNumber = "";
            this._email = "";
        }

        public ViewAccountModel(int id, string fullName, string userName, string phoneNumber, string email)
        {
            this._id = id;
            this._fullName = fullName;
            this._userName = userName;
            this._phoneNumber = phoneNumber;
            this._email = email;
        }

        public PrincipalModel toPrincipal()
        {
            PrincipalModel principal = new PrincipalModel();
            principal._id = this._id;
            principal._fullName = this._fullName;
            principal._userName = this._userName;
            principal._phoneNumber = this._phoneNumber;
            principal._credentials._email = this._email;
            return principal;
        }
    }
}