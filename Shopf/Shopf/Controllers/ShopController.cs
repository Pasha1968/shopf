using Shopf.Models.Data;
using Shopf.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop 18
        public ActionResult Index()
        {
            return RedirectToAction("Index","Page");
        }
        public ActionResult CategoryMenuPartial() {
            List<CategoryVM> categoryVMList;
            using (DB db = new DB()) {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return PartialView("_CategoryMenuPartial",categoryVMList);
        }
    }
}