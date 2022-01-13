using FallenMoon.Library;
using FallenMoon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FallenMoon.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Index()
        {
            if (Request.Cookies["fallen_token"] == null && Session["User"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.Cookies["fallen_token"] != null || Session["User"] != null)
                return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            else if (Request.UrlReferrer != null)
            {
                var referrer = Request.UrlReferrer.ToString();
                ViewBag.ReturnUrl = new Uri(referrer).AbsolutePath;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.UserEmail))
                        ModelState.AddModelError("UserEmail", "Email is required");
                    if (string.IsNullOrEmpty(model.Password))
                        ModelState.AddModelError("Password", "Password is required");

                    throw new Exception("Email and password are required");
                }

                if (Request.Cookies["fallen_token"] != null)
                {
                    var token = Request.Cookies["fallen_token"].Value;
                    if (!string.IsNullOrEmpty(token))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }

                using (Security security = new Security())
                {
                    var helper = security.LoginUser(model.UserEmail, model.Password);
                    if (helper == null || helper.LoginSuccess == false)
                        throw new Exception(helper.LoginMessage);

                    HttpCookie cookie = new HttpCookie("fallen_token", helper.UserAccount.Token);

                    if (model.RememberMe)
                        cookie.Expires = DateTime.Now.AddYears(1);
                    else
                    {
                        Session["User"] = helper.UserAccount;
                        cookie.Expires = DateTime.Now.AddDays(7);
                    }

                    Response.SetCookie(cookie);

                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (Request.Cookies["fallen_token"] != null || Session["User"] != null)
                return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            else if (Request.UrlReferrer != null)
            {
                var referrer = Request.UrlReferrer.ToString();
                ViewBag.ReturnUrl = new Uri(referrer).AbsolutePath;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (model.Password != model.ConfirmPassword)
                        throw new Exception("Passwords do not match!");
                    else
                        throw new Exception("Email, Username and password are required");
                }

                if (Request.Cookies["fallen_token"] != null)
                {
                    var token = Request.Cookies["fallen_token"].Value;
                    if (!string.IsNullOrEmpty(token))
                    {
                        throw new Exception("User already logged in");
                    }
                }

                using (Security security = new Security())
                {
                    var helper = security.RegisterUser(model.UserEmail, model.Password, model.UserName);
                    if (helper == null || helper.RegisterSuccess == false)
                        throw new Exception(helper.RegisterMessage);

                    HttpCookie cookie = new HttpCookie("fallen_token", helper.UserAccount.Token);

                    Session["User"] = helper.UserAccount;
                    cookie.Expires = DateTime.Now.AddDays(7);

                    Response.SetCookie(cookie);

                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["fallen_token"] != null)
            {
                var cookie = new HttpCookie("fallen_token");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            if (Session["User"] != null)
            {
                Session["User"] = null;
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("Profile/{userName}", Name = "Profile")]
        public ActionResult UserProfile(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Index", "Home");

            ProfileViewModel model = new ProfileViewModel();

            var helper = UserManager.GetProfile(userName);
            if (helper != null && helper.Success)
                model.Profile = helper.Profile;
            else
                return RedirectToAction("Index", "Home");

            return View(model);
        }
    }
}