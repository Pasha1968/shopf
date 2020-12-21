using Shopf.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        //[ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            UserVM data = new UserVM();
            return View(data);
        }
    }
}