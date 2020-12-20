using PagedList;
using Shopf.Models.Data;
using Shopf.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductVM model,HttpPostedFileBase file) {
            if (!ModelState.IsValid) {
                using (DB db = new DB()) {
                    model.Categories = new SelectList (db.Categories.ToList(), "Id", "Name");
                  //  model.Categories = new SelectList(db.Categories, "Id", "Name"); // add this
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
                product.CategoryId = model.CategoryId;
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x=> x.Id ==model.CategoryId);
                product.CategoryName = catDTO.Name;
                db.Products.Add(product);
                db.SaveChanges();
                id = product.Id;
            }
            TempData["SM"] = "Product added";
            #region Upload Image
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\"+ id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);
            if (file == null) {
                TempData["SM"] = "nuill";
                return View(model);
            }
            if (file != null && file.ContentLength > 0) {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png") {
                    using (DB db = new DB()) {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "No image");
                        return View(model);
                    }
                }
            }
            string ImageName = file.FileName;
            using (DB db = new DB())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.ImageName = ImageName;
                db.SaveChanges();
            }
                var path = string.Format($"{pathString2}\\{ImageName}");
                var path2 = string.Format($"{pathString3}\\{ImageName}");

                file.SaveAs(path);
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);
            // Возможно потерял "}" 15/17:05
            #endregion
            return RedirectToAction("AddProduct");
        }


        [HttpGet]
        public ActionResult Products(int? page, int? catId) {
            List<ProductVM> ListOfProductVM;

            var pageNumber = page ?? 1;

            using (DB db = new DB()) {
                ListOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();
            }
            //ViewBag- не кидать слишком много данных(Лучше моделькой тогда)
            var onePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 8);
            ViewBag.onePageOfProducts = onePageOfProducts;
            
            return View(ListOfProductVM);
        }

        [HttpGet]
        public ActionResult EditProduct(int id) {
            ProductVM model;
            using (DB db = new DB()) {
                ProductDTO dto = db.Products.Find(id);
                if (dto == null)
                {
                    return Content("This product doesn't exists");
                }
                model = new ProductVM(dto);
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file) {
            int id = model.Id;

            using (DB db = new DB()) {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                   .Select(fn => Path.GetFileName(fn));
            if (!ModelState.IsValid) {
                return View(model);
            }
            using (DB db = new DB()) {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name)) {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }
            using (DB db = new DB())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Slug.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.CategoryId = model.CategoryId;
                dto.Price = model.Price;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            
            }
            TempData["SM"] = "Product Edited";

            //Дописать логику изображений

            #region Image update;

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        ModelState.AddModelError("", "No image");
                        return View(model);
                    }
                }
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

              //var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles()) {
                    file2.Delete();
                }
                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }
                string imageName = file.FileName;
                using (DB db = new DB()){
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                file.SaveAs(path);
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(path2);


            }
            #endregion
            return RedirectToAction("EditProduct");
        }

        public ActionResult DeleteProduct(int id) {

            using (DB db = new DB()) {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString)) {
                Directory.Delete(pathString,true);

            }

            return RedirectToAction("Products");
        }

        public void SaveGalleryImages(int id) {
            foreach (string filename in Request.Files) {
                HttpPostedFileBase file = Request.Files[filename];
                if (file != null && file.ContentLength > 0) {
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
                    string pathString1 = Path.Combine(originalDirectory.ToString(),"Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1,1);
                    img.Save(path2);
                }
            }
        }
        public void DeleteImage(int id,string imageName) {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products\\" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);
            if (System.IO.File.Exists(fullPath1)) {
                System.IO.File.Delete(fullPath1);
            }
            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
        }
    }

}