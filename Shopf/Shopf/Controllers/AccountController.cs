using Shopf.Models.Data;
using Shopf.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shopf.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        //[ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            UserVM data = new UserVM();
            return View(data);
        }
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            if (!ModelState.IsValid)
                return View("CreateAccount", model);
            if (!model.Password.Equals(model.Confirm))
            {
                ModelState.AddModelError("", "Password do not match");
                return View("CreateAccount", model);
            }
            using (DB db = new DB()) {
                if(db.Users.Any(x=> x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} is taken");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };
                db.Users.Add(userDTO);
                db.SaveChanges();
                int id = userDTO.Id;
                UserRoleDTO userroleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UserRoles.Add(userroleDTO);
                db.SaveChanges();
            }
            TempData["SM"] = "Account registered";
            //UserVM data = new UserVM();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Login()
        {
            LoginUserVM data = new LoginUserVM();
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");
            return View(data);
        }
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }
            bool isValid = false;
            using (DB db = new DB()) {
                if(db.Users.Any(x=> x.UserName.Equals(model.Username)&& x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}