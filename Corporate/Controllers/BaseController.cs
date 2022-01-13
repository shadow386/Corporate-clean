using FallenMoon.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FallenMoon.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Brand = "Fallen Moon";

            base.OnActionExecuting(filterContext);
            if (Session["User"] == null && Request.Cookies["fallen_token"] != null)
            {
                var token = Request.Cookies["fallen_token"].Value;
                if (!string.IsNullOrEmpty(token))
                {
                    using (Security security = new Security())
                    {
                        var helper = security.LoginUserByToken(token);
                        if (helper.LoginSuccess)
                        {
                            Session["User"] = helper.UserAccount;
                        }
                        else
                        {
                            Response.Cookies["fallen_token"].Expires = DateTime.Now.AddDays(-1);
                        }
                    }
                }
            }
        }

        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}