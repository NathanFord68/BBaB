using BBaB.Models;
using BBaB.Services.Business;
using BBaB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBaB.Utility.Interfaces;

namespace BBaB.Controllers
{
    public class ShopController : Controller
    {
        IWeaponBusiness<WeaponModel> _weaponBusinss;
        private IBBaBLogger logger;

        public ShopController(IWeaponBusiness<WeaponModel> weaponBusiness, IBBaBLogger logger)
        {
            this._weaponBusinss = weaponBusiness;
            this.logger = logger;
        }
        public ViewResult Cart()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering Cart() in ShopController");
            try
            {
                List<WeaponModel> cartList = new List<WeaponModel>();
                ViewData["inventory"] = cartList;

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Cart View from Cart() in ShopCOntroller");
                return View("Cart");

            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "An error occured in Cart()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Cart View from Cart() in ShopController");
                return View("Cart");
            }
        }
        public ViewResult Update()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Retuning Update View from Update() in ShopController");
            return View();
        }
        public ViewResult Delete()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Delete View from Delete() in ShopController");
            return View();
        }
        [HttpGet]
        public ActionResult Shop()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering Shop in ShopController");
            try
            {
                ViewData["inventory"] = this._weaponBusinss.GetAllWeapons();

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Shop View from Shop() in ShopController");
                return View("Shop");
                
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in Shop()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Shop View from Shop() in ShopController");
                return View("Shop");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onAddToCart()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering onAddCart in ShopController");
            {
                try
                {
                    int weaponId = int.Parse(HttpContext.Request.Params.Get("weaponId"));
                    int principalId = ((PrincipalModel)HttpContext.Session["principal"])._id;

                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Adding weapon to Cart");
                    this._weaponBusinss.AddWeaponToCart(weaponId, principalId);

                    ViewData["inventory"] = this._weaponBusinss.GetAllWeapons();

                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Shop View from onAddCart() in ShopController");
                    return View("Cart");
                }
                catch (Exception e)
                {
                    this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "An error occured in onAddCart()", e);
                    ViewBag.Error = e.Message;
                    Console.WriteLine(e.StackTrace);

                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Shop View from onAddCart() in ShopController");
                    return View("Shop");
                }

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onUpdate() //TODO change the session commands to update the cart 
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering onUpdate() in ShopController");
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Update View from onUpdate() in ShopController");
                        return View("Update");
                    }


                    //HttpContext.Session.Remove("");
                    //HttpContext.Session.Add("");

                    return View("");
                }
                catch (Exception e)
                {
                    this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in onUpdate()", e);
                    ViewBag.Error = e.Message;
                    Console.WriteLine(e.StackTrace);
                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Update View from onUpdate() in ShopController");
                    return View("Update");
                }
            }

            
        }
        public ActionResult OnDelete()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering onDelete() in ShopController");
            try
            {
                HttpContext.Session.Clear();
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Index View from onDelete() in ShopController");
                return View("~/Views/Home/Index.cshtml");
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in onDelete()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning AccountInfo View from onDelete() in ShopController");
                return View("AccountInfo");
            }
        }

    }
}