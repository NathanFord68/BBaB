    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class AddressModel
    {
        public int _id { get; set; }
        public string _address1 { get; set; }
        public string _address2 { get; set; }
        public string _address3 { get; set; }
        public string _city { get; set; }
        public string _state { get; set; }
        public string _country { get; set; }
        public string _zip { get; set; }
        public PrincipalModel _resident { get; set; }

        public AddressModel()
        {
            this._id = -1;
            this._address1 = "";
            this._address2 = "";
            this._address3 = "";
            this._city = "";
            this._state = "";
            this._country = "";
            this._zip = "";
            this._resident = new PrincipalModel();
        }

        public AddressModel(int id, string address1, string address2, string address3, string city, string state, string country, string zip, PrincipalModel resident)
        {
            this._id = id;
            this._address1 = address1;
            this._address2 = address2;
            this._address3 = address3;
            this._city = city;
            this._state = state;
            this._country = country;
            this._zip = zip;
            this._resident = resident;
        }
    }
}