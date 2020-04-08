using BBaB.Models;
using BBaB.Services.Business;
using BBaB.ViewModels.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBaB.Utility.Interfaces;

namespace BBaB.Controllers
{
    public class AdminController : Controller
    {
        IWeaponBusiness<WeaponModel> _weaponService;
        private IBBaBLogger logger;

        public AdminController(IWeaponBusiness<WeaponModel> weaponService, IBBaBLogger logger)
        {
            this._weaponService = weaponService;
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult AddWeapon()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning view AddWeapon");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddWeapon(AddWeaponModel awm)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering AdminController@OnAddWeapon");
            try
            {
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Checking if ModelState if value");
                if (!ModelState.IsValid)
                {
                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "ModelState invalid, returing view AddWeapon");
                    return View("AddWeapon");
                }

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Passing weapon to business layer to be added");
                _weaponService.AddWeaponInventory(awm.ToWeapon());

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning view AddWeapon");
                return View("AddWeapon"); //TODO redirect this back to inventory screen
            }catch(Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Catching Exception", e);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning view AddWeapon");
                return View("AddWeapon");
            }
        }
    }
}