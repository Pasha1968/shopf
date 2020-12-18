using Shopf.Models.Data;
using Shopf.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        public ActionResult Categories() {
            List<CategoryVM> categoryVMList;
            using (DB db = new DB()) {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
                return View(categoryVMList);
        }

        [HttpPost]
        public string AddNewCategory(string catName) {
            string id;
            using (DB db = new DB()) {
                if (db.Categories.Any(x => x.Name == catName)) {
                    return "titletaken";
                }
                CategoryDTO dto = new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
            }
                return id;
        }
        public void ReorderCategories(int[] id)
        {
            using (DB db = new DB())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }
        public ActionResult DeleteCategory(int id)
        {
            using (DB db = new DB())
            {
                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Category deleted";
            return RedirectToAction("Categories");
        }
        [HttpPost]
        public string RenameCategory(string newCatName,int id) {
            using (DB db = new DB()) {
                if (db.Categories.Any(x => x.Name == newCatName)) {
                    return "titletaken";
                }
                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            return ("ok");
        }
        [HttpGet]
        public ActionResult AddProduct() {
            ProductVM model = new ProductVM();
            using (DB db = new DB()) {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductVM model,HttpPostedFileBase file) {
            if (!ModelState.IsValid) {
                using (DB db = new DB()) {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            using (DB db = new DB())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Name's taken");
                    return View(model);
                }
            }
            int id;
            using (DB db = new DB())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x=> x.Id ==model.CategoryId);
            }
                #region Upload Image

                #endregion
        }

    }
}