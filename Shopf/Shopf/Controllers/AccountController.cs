using Shopf.Models.Data;
using Shopf.Models.ViewModels.Account;
using Shopf.Models.ViewModels.Shop;
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
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;
            UserNavPartialVM model;
            using (DB db = new DB()) {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            return PartialView(model);
        }
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;
            UserProfileVM model;
            using (DB db = new DB())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                model = new UserProfileVM(dto);
            }
            return View("UserProfile",model);
        }
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            if (!ModelState.IsValid) {
                return View("UserProfile",model);
            }
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.Confirm))
                {
                    ModelState.AddModelError("", "Password don't match");
                    return View("UserProfile", model);
                }
            }
            using (DB db = new DB())
            {
                string userName = User.Identity.Name;
                if (userName != model.UserName)
                {
                    userName = model.UserName;
                    userNameIsChanged = true;
                }
                if(db.Users.Where(x=>x.Id !=model.Id).Any(x=>x.UserName == userName))
                {
                    ModelState.AddModelError("", $"username {model.UserName} alredy exists ");
                    model.UserName = "";
                    return View("UserProfile", model);
                }
                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.UserName = model.UserName;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                db.SaveChanges();
            }
            TempData["SM"] = "Profile Edited";
            if (!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else {
                return RedirectToAction("Logout");
            }
            
        }
        public ActionResult Orders() {
            List<OrdersForUsersVM> ordersForUser = new List<OrdersForUsersVM>();

            using (DB db = new DB()) {
                UserDTO user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                int userId = user.Id;
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();
                foreach(var order in orders)
                {
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    decimal total = 0m;
                    List<OrderDetailsDTO> orderDetailsDto = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    foreach (var orderDetails in orderDetailsDto)
                    {
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);
                        decimal price = product.Price;
                        string productName = product.Name;
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        total += orderDetails.Quantity * price;
                    }
                    ordersForUser.Add(new OrdersForUsersVM() {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        Cratedat = order.Cratedat,
                        Status = order.Status
                    });
                }
            }
            return View(ordersForUser);
        }
    }
}