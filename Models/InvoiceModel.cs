using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class InvoiceModel
    {
        public int _id { get; set; }
        public AddressModel _address { get; set; }
        public PrincipalModel _customer { get; set; }
        public float _totalValue { get; set; }
        public List<WeaponModel> _weapons { get; set; }
        public DateTime _dateTime { get; set; }

        public InvoiceModel()
        {
            this._id = -1;
            this._address = new AddressModel();
            this._customer = new PrincipalModel();
            this._totalValue = 0.0f;
            this._weapons = new List<WeaponModel>();
            this._dateTime = new DateTime();
        }

        public InvoiceModel(int id, AddressModel address, PrincipalModel customer, float totalValue, List<WeaponModel> weapons, DateTime dateTime)
        {
            this._id = id;
            this._address = address;
            this._customer = customer;
            this._totalValue = totalValue;
            this._weapons = weapons;
            this._dateTime = dateTime;
        }

    }
}