using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBaB.Models
{
    public class CartModel
    {
        public PrincipalModel _customer { get; set; }
        public List<WeaponModel> _weapons { get; set; }
        public WeaponModel _weaponToUpdate { get; set;
        }

        public CartModel()
        {
            this._customer = new PrincipalModel();
            this._weapons = new List<WeaponModel>();
            this._weaponToUpdate = new WeaponModel();
        }

        public CartModel(PrincipalModel customer, List<WeaponModel> weapons, WeaponModel weaponToUpdate)
        {
            this._customer = customer;
            this._weapons = weapons;
            this._weaponToUpdate = weaponToUpdate;
        }
    }
}