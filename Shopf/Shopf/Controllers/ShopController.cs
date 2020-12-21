using Shopf.Models.Data;
using Shopf.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult Category(string name) {
            List<ProductVM> productVMList;
            using (DB db = new DB()) {
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

                return View(productVMList);
        }

        [ActionName("product-details")]
        public ActionResult ProductDetails(string name) {

            ProductDTO dto;
            ProductVM model;
            int id = 0;
            using (DB db = new DB() ) {
                if (!db.Products.Any(x => x.Slug.Equals(name))) {
                    return RedirectToAction("Index", "Shop");
                }
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductVM(dto);
            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).Select(fn => Path.GetFileName(fn));


            return View("ProductDetails",model);
        }
    }
}