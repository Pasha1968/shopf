using Shopf.Models.Data;
using Shopf.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var page = new PageVM();
            return View(page);
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
        [HttpGet]
        // GET: Admin/Pages/Edit/id
        public ActionResult EditPage(int id) {
            PageVM model;

            using (DB db = new DB()) {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null) {
                    return Content("This page doesn't exist");
                }
                model = new PageVM(dto);
            }

                return View(model);
        }
        // POST: Admin/Pages/Edit/id
        [HttpPost]
        // GET: Admin/Pages/Edit/id
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else {
                using (DB db = new DB())
                {
                    int id = model.Id;

                    PagesDTO dto = db.Pages.Find(id);

                    dto.Title = model.Title;

                    if (model.Slug != "home") {
                        if (string.IsNullOrWhiteSpace(model.Slug)) {
                            model.Slug = model.Title.Replace(" ", "-").ToLower();
                        }
                    }
                    if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                    {
                        ModelState.AddModelError("", "That title exist");
                        return View(model);
                    }
                    dto.Body = model.Body;
                    dto.Sorting = model.Sorting;
                    dto.Slug = model.Slug;
                    dto.HasSidebar = model.HasSidebar;

                    db.SaveChanges();
                }

                return RedirectToAction("EditPage");
            }
        }

        public ActionResult PageDetails(int id) {
            PageVM model;
            using (DB db = new DB())
            {
                PagesDTO dto = db.Pages.Find(id);
                if (dto == null) {
                    return Content("The page doesn't exist");

                }
                model = new PageVM(dto);

            }
                return View(model);
        }

        public ActionResult DeletePage(int id) {
            using (DB db = new DB())
            {
                PagesDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Page deleted";
            return RedirectToAction("index");
        }


        [HttpPost]
        public void ReorderPages(int [] id)
        {
            if (id == null){
                id = new int[0];
            }
            using (DB db = new DB())
            {
                int count = 1;
                PagesDTO dto;
                foreach (var PageId in id) {
                    dto = db.Pages.Find(PageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        [HttpGet]
        public ActionResult EditSidebar() {
            SidebarVM model;

            using (DB db = new DB()) {
                //Говнокод!!!!!!!!!!
                SidebarDTO dto = db.Sidebars.Find(1);
                model = new SidebarVM(dto);
            }

                return View(model);
        }
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (DB db = new DB())
            {
                SidebarDTO dto = db.Sidebars.Find(1);// Говнокодищеееееее!!!!!

                dto.Body = model.Body;
                db.SaveChanges();
            }
            TempData["SM"] = "Edited sidebar";

            return RedirectToAction("EditSidebar");
        }

    }
}