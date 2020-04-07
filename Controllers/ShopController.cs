using BBaB.Models;
using BBaB.Services.Business;
using BBaB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBaB.Controllers
{
    public class ShopController : Controller
    {
        IWeaponBusiness<WeaponModel> _weaponBusinss;

        public ShopController(IWeaponBusiness<WeaponModel> weaponBusiness)
        {
            this._weaponBusinss = weaponBusiness;
        }
        public ViewResult Cart()
        {
            try
            {
                List<WeaponModel> cartList = new List<WeaponModel>();
                ViewData["inventory"] = cartList;

                return View("Cart");

            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);

                return View("Cart");
            }
        }
        public ViewResult Update()
        {
            return View();
        }
        public ViewResult Delete()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Shop()
        {
            try
            {
                ViewData["inventory"] = _weaponBusinss.GetAllWeapons();
                
                return View("Shop");
                
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);

                return View("Shop");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onCart()
        {
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return View("Cart");
                    }
                    //_accountService.RegisterUser(user);

                    return View("~/Views/Home/Index.cshtml");
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    Console.WriteLine(e.StackTrace);

                    return View("Cart");
                }

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onUpdate() //TODO change the session commands to update the cart 
        {
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return View("Update");
                    }


                    //HttpContext.Session.Remove("");
                    //HttpContext.Session.Add("");

                    return View("");
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    Console.WriteLine(e.StackTrace);
                    return View("Update");
                }
            }

            
        }
        public ActionResult OnDelete()
        {

            try
            {
                HttpContext.Session.Clear();
                return View("~/Views/Home/Index.cshtml");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                return View("AccountInfo");
            }
        }

    }
}