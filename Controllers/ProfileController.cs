using BBaB.Models;
using BBaB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBaB.Service.Business;
using BBaB.ViewModels.Profile;

namespace BBaB.Controllers
{
    //TODO find a way to log error stacks.
    public class ProfileController : Controller
    {
        IAccountBusiness<PrincipalModel, AddressModel> _accountBusiness;

        public ProfileController(IAccountBusiness<PrincipalModel, AddressModel> accountBusiness)
        {
            this._accountBusiness = accountBusiness;
        }
        public ViewResult Login()
        {
            return View();
        }

        public ViewResult Register()
        {
            return View("Registration");
        }

        public ViewResult ViewAccount()
        {
            PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
            ViewAccountModel vam = new ViewAccountModel(principal._id, principal._fullName, principal._userName, principal._phoneNumber, principal._credentials._email);
            return View("ViewAccount", vam);
        }

        public ActionResult Update()
        {
            PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
            UpdateProfileModel upm = new UpdateProfileModel(
                principal._fullName,
                principal._credentials._email,
                principal._phoneNumber,
                principal._userName);
            return View("UpdateAccount", upm);
        }

        public ActionResult UpdatePassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnRegister(RegistrationModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Registration");
                }

                PrincipalModel principal = new PrincipalModel();
                principal = user.ToPrincipal();
                _accountBusiness.RegisterAccount(principal);

                return View("Registration");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);

                return View("Registration");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnLogin(LoginModel model)
        {
            try
            {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }
            PrincipalModel principal = model.toPrincipal();
            principal = _accountBusiness.AuthenticateAccount(principal);

            HttpContext.Session.Add("isLoggedIn", true);
            HttpContext.Session.Add("principal", principal);

                ViewAccountModel vam = new ViewAccountModel(
                    principal._id,
                    principal._fullName,
                    principal._userName,
                    principal._phoneNumber,
                    principal._credentials._email);

            return View("ViewAccount", vam);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                return View("Login");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult OnUpdateAccount(UpdateProfileModel upm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("UpdateAccount");
                }

                PrincipalModel principal = (PrincipalModel)HttpContext.Session["Principal"];
                principal = upm.toPrincipal(principal);
                _accountBusiness.UpdateAccount(principal);

                HttpContext.Session.Remove("principal");
                HttpContext.Session.Add("Principal", principal);

                return View("ViewAccount");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                return View("UpdateAccount");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnPasswordUpdate(UpdatePasswordModel upm)
        {
            try
            {
                PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
                principal._credentials._password = upm._oldPassword;
                principal = _accountBusiness.UpdatePassword(principal, upm._newPassword);

                HttpContext.Session.Remove("principal");
                HttpContext.Session.Add("principal", principal);
                return View("ViewAccount");
            }catch(Exception e)
            {
                return View("UpdatePassword");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnDelete(ViewAccountModel model)
        {
            try
            {
                PrincipalModel principal = model.toPrincipal();
                _accountBusiness.DeleteAccount(principal);
                HttpContext.Session.Clear();
                return View("Registration");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                return View("ViewAccount");
            }
        }

        public RedirectToRouteResult OnLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Profile");
        }
    }
}