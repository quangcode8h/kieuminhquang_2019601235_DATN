using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;
using Watch.Models.DTO;
using Watch.Models.EF;

namespace Watch.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private const string CartSession = "cart";
        // GET: Cart
        public ActionResult Index()
        {
            //hiển thị giỏ        
            var cart = Session[CartSession];
            var list = new List<CartDTO>();

            if (cart != null)
            {
                list = (List<CartDTO>)cart;
            }
            return View(list);
        }


        public JsonResult AddCart(long product_ID, int quantity)
        {
            var product = new ProductBusiness().findID(product_ID);
            var cart = Session[CartSession];
            if (cart != null)//Nếu giỏ đã chứa sản phẩm
            {
                var list = (List<CartDTO>)cart;
                if (list.Exists(x => x.Product.ID == product_ID))//nếu giỏ đã chứa sản phẩm có ID = BookID
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == product_ID)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //Tạo đối tượng mới
                    var item = new CartDTO();
                    item.Product = product;
                    item.Quantity = quantity;
                    item.actual_number = (int)product.Quantity;
                    list.Add(item);
                }
            }
            else//nếu giỏ hàng trống
            {
                var item = new CartDTO();
                item.Product = product;
                item.Quantity = quantity;
                item.actual_number = (int)product.Quantity;
                var list = new List<CartDTO>();
                list.Add(item);

                Session[CartSession] = list;
            }
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        //Xóa từng sản phẩm
        public JsonResult Delete(long id)
        {
            var cartSec = (List<CartDTO>)Session[CartSession];
            cartSec.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = cartSec;
            return Json(new
            {
                status = true
            });
        }

        //Sửa số lượng sp trong giỏ hàng
        public JsonResult Edit(long product_ID, int quantity)
        {
            var productSec = (List<CartDTO>)Session[CartSession];

            foreach (var item in productSec)
            {
                if (item.Product.ID == product_ID)
                {
                    item.Quantity = quantity;
                }

            }

            Session[CartSession] = productSec;
            return Json(new
            {
                status = true
            });
        }


        public ActionResult Payment()
        {
            if (Session["user"] == null)
            {
                TempData["error"] = "Bạn vui lòng đăng nhập để đặt hàng.";
                Session["check_payment"] = true;
                return Redirect("/user/login");
            }
            Session["check_payment"] = null;
            return View();
        }

        //Thanh toán hóa đơn
        [HttpPost]
        public ActionResult OrderPayment(Watch.Models.EF.Order order)
        {
            var cart = (List<CartDTO>)Session[CartSession];

            var res = new OrderBusiness().addOrder(order);
            decimal? TotalMoney = 0;
            int Quantity = 0;
            if (res)
            {
                foreach (var item in cart)
                {

                    var od = new Order_Detail();
                    od.Product_ID = item.Product.ID;
                    od.Quantity = item.Quantity;
                    od.Order_ID = new OrderBusiness().findMaxID();
                    if (item.Product.Price != null)
                    {
                        od.Price = item.Product.Price;
                        od.Amount = (int)item.Product.Price * item.Quantity;
                        TotalMoney += item.Product.Price * item.Quantity;
                    }
                    else
                    {
                        od.Price = item.Product.Promotion_Price;
                        od.Amount = (int)item.Product.Promotion_Price * item.Quantity;
                        TotalMoney += item.Product.Promotion_Price * item.Quantity;
                    }
                    Quantity += item.Quantity;
                    new OrderBusiness().addOrder_Detail(od);

                    //update tổng tiền và số lượng 
                    new OrderBusiness().Update_Order((long)od.Order_ID, TotalMoney, Quantity);

                }

            }
            else
            {
                return Redirect("/cart/payment");
            }
            Session[CartSession] = null;
            return Redirect("/cart/success");
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}