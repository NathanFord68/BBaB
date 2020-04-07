using BBaB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BBaB.ViewModels.Weapon
{
    public class AddWeaponModel
    {
        [Display(Name ="Make")]
        [Required(ErrorMessage ="This is a required field")]
        [StringLength(50, ErrorMessage ="Make cannot be longer than 50 charcters.")]
        public string _make { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "This is a required field")]
        [StringLength(50, ErrorMessage = "Model cannot be longer than 50 charcters.")]
        public string _model { get; set; }

        [Display(Name = "Caliber")]
        [Required(ErrorMessage = "This is a required field")]
        [StringLength(50, ErrorMessage = "Caliber cannot be longer than 10 charcters.")]
        public string _caliber { get; set; }

        [Display(Name = "Serial Number")]
        [Required(ErrorMessage = "This is a required field")]
        [StringLength(50, ErrorMessage = "Serial cannot be longer than 25 charcters.")]
        public string _serial { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "This is a required field")]
        public double _price { get; set; }

        public AddWeaponModel()
        {
            this._make = "";
            this._model = "";
            this._caliber = "";
            this._serial = "";
            this._price = 0.0f;
        }

        public AddWeaponModel(string make, string model, string caliber, string serial, double price)
        {
            this._make = make;
            this._model = model;
            this._caliber = caliber;
            this._serial = serial;
            this._price = price;
        }

        public WeaponModel ToWeapon()
        {
            return new WeaponModel(-1, this._make, this._model, this._caliber, this._serial, this._price);
        }

    }
}