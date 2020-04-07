using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class PrincipalModel
    {
        public int _id { get; set; }
        public string _fullName { get; set; }
        public string _userName { get; set; }
        public string _phoneNumber { get; set; }
        public bool _accountConfirmed { get; set; }
        public string _salt { get; set; }
        public int _adminLevel { get; set; }
        public string _adminTitle { get; set; }
        public CredentialModel _credentials { get; set; }

        public PrincipalModel()
        {
            this._id = -1;
            this._fullName = "";
            this._userName = "";
            this._phoneNumber = "";
            this._accountConfirmed = false;
            this._salt = "";
            this._adminLevel = 0;
            this._adminTitle = "User";
            this._credentials = new CredentialModel("", "");
        }

        public PrincipalModel(int id, string fullName, string userName, string phoneNumber, bool accountConfirmed, string salt, int adminLevel, string adminTitle, CredentialModel credentials)
        {
            this._id = id;
            this._fullName = fullName;
            this._userName = userName;
            this._phoneNumber = phoneNumber;
            this._accountConfirmed = accountConfirmed;
            this._salt = salt;
            this._adminLevel = adminLevel;
            this._adminTitle = adminTitle;
            this._credentials = credentials;
        }

    }
}