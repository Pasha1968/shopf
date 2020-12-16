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
        [HttpGet]
        public ActionResult AddPage()
        {
            
            return View();
        }
        // Post: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Чек валидности ->  переменная заголовка(Slug) -> Инит Page DTO -> дать заголовок и т.д -> save в БД
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (DB db = new DB()) {
                string slug;
                PagesDTO dto = new PagesDTO();
                dto.Title = model.Title.ToUpper();
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title exist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug)) {
                    ModelState.AddModelError("", "That Slug exist");
                    return View(model);
                }
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Page has been added";
            return RedirectToAction("Index");
        }
    }
}