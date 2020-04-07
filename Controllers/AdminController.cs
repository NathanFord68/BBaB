using BBaB.Models;
using BBaB.Services.Business;
using BBaB.ViewModels.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBaB.Controllers
{
    public class AdminController : Controller
    {
        IWeaponBusiness<WeaponModel> _weaponService;

        public AdminController(IWeaponBusiness<WeaponModel> weaponService)
        {
            this._weaponService = weaponService;
        }

        [HttpGet]
        public ActionResult AddWeapon()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddWeapon(AddWeaponModel awm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("AddWeapon");
                }

                _weaponService.AddWeaponInventory(awm.ToWeapon());


                return View("AddWeapon"); //TODO redirect this back to inventory screen
            }catch(Exception e)
            {
                return View("AddWeapon");
            }
        }
    }
}