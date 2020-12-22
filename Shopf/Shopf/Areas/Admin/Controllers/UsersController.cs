using Shopf.Models.Data;
using Shopf.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Users()
        {
            List<UserVM> list;
            using (DB db = new DB())
            {
                list = db.Users.ToArray().OrderBy(x => x.Id).Select(x => new UserVM(x)).ToList();
            }
            return View(list);
        }
        public ActionResult DeleteUser(int id)
        {
            using (DB db = new DB())
            {
                UserDTO dto = db.Users.Find(id);
                db.Users.Remove(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "User deleted";
            return RedirectToAction("Users");
        }
    }
}