using BBaB.Models;
using BBaB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBaB.Service.Business;
using BBaB.ViewModels.Profile;
using BBaB.Utility.Interfaces;

namespace BBaB.Controllers
{
    //TODO find a way to log error stacks.
    public class ProfileController : Controller
    {
        IAccountBusiness<PrincipalModel, AddressModel> _accountBusiness;
        private IBBaBLogger logger;

        public ProfileController(IAccountBusiness<PrincipalModel, AddressModel> accountBusiness, IBBaBLogger logger)
        {
            this._accountBusiness = accountBusiness;
            this.logger = logger;
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering ProfileController");
        }
        public ViewResult Login()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Login View from Login() in ProfileController");
            return View();
        }

        public ViewResult Register()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Registration View from Register() in ProfileController");
            return View("Registration");
        }

        public ViewResult ViewAccount()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering ViewAccount() in ProfileController");
            PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
            ViewAccountModel vam = new ViewAccountModel(principal._id, principal._fullName, principal._userName, principal._phoneNumber, principal._credentials._email);
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning ViewAccount View from ViewAccount() in ProfileController");
            return View("ViewAccount", vam);
        }

        public ActionResult Update()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering Update() in ProfileController");
            PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Populating an UpdateProfileModel");
            UpdateProfileModel upm = new UpdateProfileModel(
                principal._fullName,
                principal._credentials._email,
                principal._phoneNumber,
                principal._userName);
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning UpdateAccount View from Update() in ProfileController with an UpdatedProfileModel");
            return View("UpdateAccount", upm);
        }

        public ActionResult UpdatePassword()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning UpdatePassword View from UpdatePassword() in ProfileController");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnRegister(RegistrationModel user)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering onRegister() in ProfileController");
            try
            {
                if (!ModelState.IsValid)
                {
                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Registration View from onRegister() in ProfileController");
                    return View("Registration");
                }

                PrincipalModel principal = new PrincipalModel();
                principal = user.ToPrincipal();
                _accountBusiness.RegisterAccount(principal);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Login View from onRegister() in ProfileController");
                return View("Login");
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in onRegister()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Registration View from onRegister() in ProfileController");
                return View("Registration");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnLogin(LoginModel model)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering onLogin in ProfileController");
            try
            {
            if (!ModelState.IsValid)
            {
                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Login View from onLogin() in ProfileCOntroller");
                    return View("Login");
            }
            PrincipalModel principal = model.toPrincipal();
            principal = _accountBusiness.AuthenticateAccount(principal);

            HttpContext.Session.Add("isLoggedIn", true);
            HttpContext.Session.Add("principal", principal);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Populating a ViewAcountModel");
                ViewAccountModel vam = new ViewAccountModel(
                    principal._id,
                    principal._fullName,
                    principal._userName,
                    principal._phoneNumber,
                    principal._credentials._email);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning ViewAccount View from onLogin() in ProfileController with ViewAccountModel");
                return View("ViewAccount", vam);
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in onLogin()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning Login View from onLogin() in ProfileController");
                return View("Login");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult OnUpdateAccount(UpdateProfileModel upm)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering OnUpdateAccount() in ProfileController");
            try
            {
                if (!ModelState.IsValid)
                {
                    this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning UpdateAccount View from OnUdateAccount() in ProfileController");
                    return View("UpdateAccount");
                }

                PrincipalModel principal = (PrincipalModel)HttpContext.Session["Principal"];
                principal = upm.toPrincipal(principal);
                _accountBusiness.UpdateAccount(principal);

                HttpContext.Session.Remove("principal");
                HttpContext.Session.Add("Principal", principal);

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Populating a ViewAccountModel");
                ViewAccountModel vam = new ViewAccountModel(principal._id,
                    principal._fullName,
                    principal._userName,
                    principal._phoneNumber,
                    principal._credentials._email);

                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning ViewAcount View from OnUpdateAccount() in ProfileController with a ViewAccountModel");
                return View("ViewAccount", vam);
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in OnUpdateAccount()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                return View("UpdateAccount");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnPasswordUpdate(UpdatePasswordModel upm)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering OnPasswordUpdate() in ProfileController");
            try
            {
                PrincipalModel principal = (PrincipalModel)HttpContext.Session["principal"];
                principal._credentials._password = upm._oldPassword;
                principal = _accountBusiness.UpdatePassword(principal, upm._newPassword);

                HttpContext.Session.Remove("principal");
                HttpContext.Session.Add("principal", principal);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning ViewAccount View from OnPAsswordUpdate() in ProfileController");
                return View("ViewAccount");
            }catch(Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in OnPasswordUpdate()", e);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning UpdatePassword View from OnPasswordUpdate() in ProfileController");
                return View("UpdatePassword");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnDelete(ViewAccountModel model)
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Enterinn OnDelete() in ProfileController");
            try
            {
                PrincipalModel principal = model.toPrincipal();
                _accountBusiness.DeleteAccount(principal);
                HttpContext.Session.Clear();
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Retuning Registration View from OnDelete() in ProfileController");
                return View("Registration");
            }
            catch (Exception e)
            {
                this.logger.Error(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Error occured in OnDelete()", e);
                ViewBag.Error = e.Message;
                Console.WriteLine(e.StackTrace);
                this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning ViewAccount View from OnDelete() in ProfileController");
                return View("ViewAccount");
            }
        }

        public RedirectToRouteResult OnLogout()
        {
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Entering OnLogout() in ProfileController");
            HttpContext.Session.Clear();
            this.logger.Info(((PrincipalModel)HttpContext.Session["principal"])._credentials._email, "Returning a Login and Profile RedirectToAction from OnLogout() in ProfileController");
            return RedirectToAction("Login", "Profile");
        }
    }
}