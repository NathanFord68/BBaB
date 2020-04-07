using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class CredentialModel
    {
        public string _email { get; set; }
        public string _password { get; set; }

        public CredentialModel()
        {
            this._email = "";
            this._password = "";
        }

        public CredentialModel(string email, string password)
        {
            this._email = email;
            this._password = password;
        }
    }
}