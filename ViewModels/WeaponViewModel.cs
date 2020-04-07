using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels
{
    public class WeaponViewModel
    {
        [Display(Name = "Make")]
        [Required(ErrorMessage = "This is a required field")]
        public string _make { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "This is a required field")]
        public string _model { get; set; }

        [Display(Name = "Caliber")]
        [Required(ErrorMessage = "This is a required field")]
        public string _caliber { get; set; }

        [Display(Name = "Serial Number")]
        [Required(ErrorMessage = "This is a required field")]
        public string _serialNumber { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "This is a required field")]
        public float _price { get; set; }

        public WeaponViewModel()
        {
            this._make = "";
            this._model = "";
            this._caliber = "";
            this._serialNumber = "";
            this._price = 0.0f;
        }

        public  WeaponViewModel(string make, string model, string caliber, string serialNumber, float price)
        {
            this._make = make;
            this._model = model;
            this._caliber = caliber;
            this._serialNumber = serialNumber;
            this._price = price;
        }

    }
}