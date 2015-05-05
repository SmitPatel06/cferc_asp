using cferc_website.Models;
using cferc_website.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace cferc_website.Controllers
{
    public class AccountsController : Controller
    {
        private userEntities db = new userEntities();

        public ActionResult LogOn()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LogOn(userTable model, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                string userName = model.userName;
                string userpass = model.userPassword;

                bool valid = db.userTables.Any(u => u.userName == userName && u.userPassword == userpass);    

                if (valid)
                {
                    FormsAuthentication.SetAuthCookie(model.userName, true);
                    if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



    }


}
