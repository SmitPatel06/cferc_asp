using cferc_website.Models;
using cferc_website.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            var user = (from records in db.userTables
                        where records.userName == model.userName
                        select new { userName = records.userName, userPassword = records.userPassword });

            if (user.Count() == 0) { ModelState.AddModelError("", "The user name or password provided is incorrect."); }

            if(string.IsNullOrEmpty(model.userName))
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect");
            }

            if(string.IsNullOrEmpty(model.userPassword))
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect");
            }

            if(ModelState.IsValid)
            {
                                

                bool valid = security.validatePassword(model.userPassword, user.First().userPassword);    

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

        [Authorize]
        public ActionResult ChangePassword()
        {
            string currentUserName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            changePassViewModel ViewModel = new changePassViewModel();
            var getUser = (from records in db.userTables
                           where records.userName == currentUserName
                           select records);
            ViewModel.user = getUser.First();

            return View(ViewModel);
        }


        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(changePassViewModel pModel)
        {
            string userName = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            bool valid = false;
            userTable user = new userTable();

            //validate old password
            if (string.IsNullOrEmpty(pModel.oldPassInput))
            {
                ModelState.AddModelError("oldPassInput", "Password is Empty");
            }

            if (ModelState.IsValid)
            {
                
                var getUser = (from records in db.userTables
                               where records.userName == userName
                               select records);
                user = getUser.FirstOrDefault();
                valid = security.validatePassword(pModel.oldPassInput, user.userPassword);
            }

            if (!valid)
            {
                ModelState.AddModelError("oldPassInput", "Invalid Password");
            }

            if (pModel.newPassInput != pModel.newPassInput2)
            {
                ModelState.AddModelError("newPassInput", "Passwords must match.");
            }
            else if (string.IsNullOrEmpty(pModel.newPassInput2))
            {
                ModelState.AddModelError("newPassInput2", "Password is Empty");
            }

            if (!string.IsNullOrEmpty(pModel.newPassInput))
            {
                string regex = @"(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}";
                Regex re = new Regex(regex);
                if (!re.IsMatch(pModel.newPassInput))
                {
                    ModelState.AddModelError("newPassInput", "Invalid password. Password must be at least 8 characters with at least one lowercase, one uppercase letter, and one digit.");
                }
            }
            else
            {
                ModelState.AddModelError("userPasswordFirst", "Password is Empty");
            }

            if (ModelState.IsValid)
            {
                string passHash = security.createHash(pModel.newPassInput2);
                user.userPassword = passHash;
                db.SaveChanges();
                return RedirectToAction("passwordChanged", "Accounts");
            }
            pModel.user = user;
            pModel.user.userName = userName;

            return View(pModel);
            
        }

        [Authorize]
        public ActionResult passwordChanged()
        {
            return View();
        }



    }


}
