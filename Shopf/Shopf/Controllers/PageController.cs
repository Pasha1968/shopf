using Shopf.Models.Data;
using Shopf.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Index( string page = "")
        {
            if (page == "")
                page = "home";

            PageVM model;
            PagesDTO dto;
            using (DB db = new DB()) {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });
            }
            using (DB db = new DB())
            {
                dto = db.Pages.Where(x=>x.Slug == page).FirstOrDefault();
            }
            ViewBag.PageTitle = dto.Title;

            if(dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            model = new PageVM(dto);


            return View(model);
        }
        public ActionResult PagesMenuPartial() {
            List<PageVM> pageVMList;
            using (DB db = new DB()) {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            return PartialView("_PagesMenuPartial", pageVMList);
        }
        public ActionResult SidebarPartial() {
            SidebarVM model;
            using (DB db = new DB()) {
                SidebarDTO dto = db.Sidebars.Find(1);// Говнокод
                model = new SidebarVM(dto);
            }
            return PartialView("_SidebarPartial", model);
        }
    }
}