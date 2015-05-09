using cferc_website.Models;
using cferc_website.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace cferc_website.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private userEntities db = new userEntities();

        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            
            return View();
        }

        [Authorize(Roles="admin")]
        public ActionResult Add()
        {
            List<role> roles = new List<role>();
            roles.Add(new role(1, "user"));
            roles.Add(new role(2, "admin"));
            adminViewModel model = new adminViewModel(roles);
           
            
            return View(model);
        }

        [HttpPost][Authorize(Roles="admin")]
        public ActionResult Add(adminViewModel pModel)
        {
            
            //super hackish but will work until roles table gets built in database
            List<role> roles = new List<role>();
            roles.Add(new role(1, "user"));
            roles.Add(new role(2, "admin"));
            pModel.roles = roles;
            string passHash;
            userTable newUser = new userTable();

            //Validation
            var existingUser = (from records in db.userTables
                                where records.userName == pModel.userName
                                select new { uName = records.userName });
            
            if (existingUser.Count() != 0)
            {
                ModelState.AddModelError("userName", "User Name already exists, pick another User Name");
            }
            
            

            if (!string.IsNullOrEmpty(pModel.userName))
            {
                string regex = "([a-zA-Z0-9]){5,10}";
                Regex re = new Regex(regex);
                if (!re.IsMatch(pModel.userName))
                {
                    ModelState.AddModelError("userName", "Invalid User Name: User Name must be 5-10 characters and may only contain alphanumeric characters.");
                }
            }
            else
            {
                ModelState.AddModelError("userName", "User Name is Empty.");
            }

            if (!string.IsNullOrEmpty(pModel.userPasswordFirst))
            {
                string regex = @"(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}";
                Regex re = new Regex(regex);
                if (!re.IsMatch(pModel.userPasswordFirst))
                {
                    ModelState.AddModelError("userPasswordFirst", "Invalid password. Password must be at least 8 characters with at least one lowercase, one uppercase letter, and one digit.");
                }
            }
            else
            {
                ModelState.AddModelError("userPasswordFirst", "Password is Empty");
            }

            
            if (pModel.userPasswordFirst != pModel.userPasswordSecond)
            {
                ModelState.AddModelError("userPasswordSecond", "Passwords do not match");
            }
            else if(string.IsNullOrEmpty(pModel.userPasswordSecond))
            {
                ModelState.AddModelError("userPasswordSecond", "Password is Empty");
            }

            //add data

            if (ModelState.IsValid)
            {
                passHash = security.createHash(pModel.userPasswordFirst);
                newUser.userName = pModel.userName;
                if (pModel.selectedRoleID == 1) { newUser.userRole = "user"; } else { newUser.userRole = "admin"; }
                newUser.userPassword = passHash;
                db.userTables.Add(newUser);
                db.SaveChanges();
            }
            
            return View(pModel);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Update()
        {
            changePassAdminViewModel model = new changePassAdminViewModel(db.userTables.ToList());
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Update(changePassAdminViewModel pModel)
        {
            string userName = (from records in db.userTables
                               where records.userID == pModel.selectedUserId
                               select records.userName).FirstOrDefault();

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


            changePassAdminViewModel model = new changePassAdminViewModel(db.userTables.ToList());
            return View(model);
            
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete()
        {
            deleteViewModel model = new deleteViewModel(db.userTables.ToList());
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Delete(deleteViewModel pModel)
        {
            userTable userToDelete = (from records in db.userTables
                                      where records.userID == pModel.selectedUserId
                                      select records).FirstOrDefault();
            ViewBag.user = userToDelete.userName;
            db.userTables.Remove(userToDelete);
            db.SaveChanges();

            return RedirectToAction("DeleteConfirmed", "Admin");

        }

        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed()
        {
            return View();
        }

    }
}
