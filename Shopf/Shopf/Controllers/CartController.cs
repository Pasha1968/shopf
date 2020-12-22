using Shopf.Models.Data;
using Shopf.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Shopf.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            if(cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Massage = "You're cart is Empty";
                return View();
            }
            decimal total = 0m;
            foreach(var item in cart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total;
            return View(cart);
        }
        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();

            int qty = 0;

            decimal price = 0m;

            if (Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"]; // Компилятор думает, что тут обьект, а мы говорим ЛИст:)
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else {
                model.Quantity = 0;
                model.Price = 0m;
            }

            return PartialView("_CartPartial",model);
        }
    
        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = new CartVM();

            using (DB db = new DB())
            {
                ProductDTO product = db.Products.Find(id);
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else {
                    productInCart.Quantity++;
                }
            }
            int qty = 0;
            decimal price = 0m;
            foreach(var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }
            model.Quantity = qty;
            model.Price = price;
            Session["cart"] = cart;
            return PartialView("_AddToCartPartial", model);
        }
        
        public JsonResult IncrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (DB db = new DB()) {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                model.Quantity++;
                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result,JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (DB db = new DB())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                if (model.Quantity > 1)
                    model.Quantity--;
                else {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //return View();
        }
        public void RemoveProduct(int productId) {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (DB db = new DB())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                cart.Remove(model);
            }
        }
        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            return PartialView(cart);
        }

            [HttpPost]
        public ActionResult PlaceOrder()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            string userName = User.Identity.Name;
            int orderId = 0;
            string userMail = "";
            string status = "Accepted";
            //Бессконечный запро
            using (DB db = new DB())
            {
                OrderDTO orderDto = new OrderDTO();
                var q = db.Users.FirstOrDefault(x => x.UserName == userName);
                int userId = q.Id;
                userMail = q.EmailAdress;
                // // // Норм
                orderDto.UserId = userId;
                orderDto.Cratedat = DateTime.Now;
                orderDto.Status = status;
                db.Orders.Add(orderDto);
                db.SaveChanges();//Асинхронно можно поймать эррор, так что ну его в баню
                ////// ДО
                orderId = orderDto.OrderId;
                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();
                foreach (var item in cart)
                {
                    if (item == null || cart ==null) {
                        break;
                    }
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = item.ProductId;
                    orderDetailsDto.Quantity = item.Quantity;
                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                    if (item == null || cart == null)
                    {
                        break;
                    }
                }

            }
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                //Почта (Письмо заказа)
                Credentials = new NetworkCredential("a766ee98be9a76", "d0200c0f7aafd1"),
                EnableSsl = true
            };
            client.Send("shop@example.com", $"{userMail}", $"New Order№{orderId}", $"You have created a new order№{orderId}");
            Session["cart"] = null;//ООбновить сессию
            return RedirectToAction("Index", "Shop");
        }

    }
}