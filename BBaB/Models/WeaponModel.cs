using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class WeaponModel
    {
        public int _id { get; set; }
        public string _make { get; set; }
        public string _model { get; set; }
        public string _caliber { get; set; }
        public string _serialNumber { get; set; }
        public double _price { get; set; }

        public WeaponModel()
        {
            this._id = -1;
            this._make = "";
            this._model = "";
            this._caliber = "";
            this._serialNumber = "";
            this._price = 0.0f;
        }

        public WeaponModel(int id, string make, string model, string caliber, string serialNumber, double price)
        {
            this._id = id;
            this._make = make;
            this._model = model;
            this._caliber = caliber;
            this._serialNumber = serialNumber;
            this._price = price;
        }
    }
}