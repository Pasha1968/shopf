using Shopf.Models.Data;
using Shopf.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using (DB db = new DB()) {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
                return View(pageList);
        }
        // GET: Admin/Pages/AddPage
        public ActionResult AddPage()
        {
            
            return View();
        }
    }
}