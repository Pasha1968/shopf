using Shopf.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();

            int qty = 0;

            decimal price = 0m;

            if(Session["cart"]!= null)

            return PartialView();
        }
    }
}