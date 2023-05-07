
using Watch.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.DTO;
using Watch.Models.Business;

namespace Watch.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Home
        [CustomRoleProvider(RoleName = new string[] { "Admin", "Boss", "Staff", "Editor" })]
        public ActionResult Index()
        {
            var admin = (Manager) Session["admin"];
            if(admin.RoleID == 2)
            {
                return RedirectToAction("Index", "Product");
            }
            if (admin.RoleID  == 3)
            {
                return RedirectToAction("Index", "Brand");
            }
            var sell = from detail in db.Order_Detail
                       join order in db.Orders on detail.Order_ID equals order.ID
                       where order.Status == 3 || order.Payment == 1
                       select new
                       {
                           detail.Quantity,
                           detail.Amount
                       };
            ViewBag.Count_sell = db.Orders.Where(x => x.Payment == 1 || x.Status == 3).Sum(x => x.TotalQuantity);

            //Thống kê doanh thu
            ViewBag.Count_money = db.Orders.Where(x => x.Payment == 1 || x.Status == 3).Sum(x => x.TotalMoney);

            //Doanh thu hôm nay
            ViewBag.Money_today = db.Orders.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now) &&
                                                        x.Payment == 1 ||
                                                        DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now) &&
                                                        x.Status == 3).Select(x => x.TotalMoney).Sum();
            //Thống kê đơn đặt hàng
            ViewBag.Count_Order = db.Orders.Count();


            //Thống kê tồn ko
            ViewBag.quantity_product = db.Products.Where(x => x.Quantity > 0).OrderByDescending(x => x.Quantity).Take(10).ToList();

            //Thống kê lượng hàng bán ra
            var listProduct_sell = new List<Order_DetailDTO>();
            foreach (var item in db.Order_Detail.Where(x => x.Order.Payment == 1 || x.Order.Status == 3).ToList())
            {

                if (listProduct_sell.Exists(x => x.Product_ID == item.Product_ID))
                {
                    foreach (var jtem in listProduct_sell)
                    {
                        if (jtem.Product_ID == item.Product_ID)
                        {
                            jtem.Quantity += item.Quantity;
                            jtem.Amount += item.Amount;
                        }
                    }

                }
                else
                {
                    var productsell = new Order_DetailDTO();
                    productsell.Product_Name = item.Product.Product_Name;
                    productsell.Product_ID = item.Product_ID;
                    productsell.Quantity = item.Quantity;
                    productsell.Amount = item.Amount;
                    listProduct_sell.Add(productsell);
                }
            }

            ViewBag.product_sell = listProduct_sell.OrderByDescending(x => x.Quantity).Take(10).ToList();

            //Thống kê đánh giá hôm nay
            ViewBag.Review_today = db.Reviews.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê đơn đạt hàng hôm nay
            var Order_today = from order in db.Orders
                              select new
                              {
                                  order.ID,
                                  order.Payment,
                                  order.Status,
                                  order.CreatedDate
                              };
            ViewBag.Order_today = Order_today.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Đơn hàng đã thanh toán
            ViewBag.Order_Payment = Order_today.Where(x => x.Payment == 1 || x.Status == 3).Count();

            //Đơn hàng đang giao
            ViewBag.Ordering = Order_today.Where(x => x.Status == 2).Count();

            //Đơn hàng đã hủy
            ViewBag.Order_Cancer = Order_today.Where(x => x.Status == 0).Count();

            //Đơn hàng đã từ chối nhận
            ViewBag.Order_refuse = Order_today.Where(x => x.Status == -1).Count();
            return View();
        }


        public ActionResult TotalSaleMonth()
        {
            int[] month = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var lstTotal = new List<TotalSale>();
            for (int i = 1; i <= 12; i++)
            {
                var model = db.Orders.Where(x =>
                                                x.CreatedDate.Value.Month == i &&
                                                x.Payment == 1 ||
                                                x.CreatedDate.Value.Month == i &&
                                                x.Status == 3).Sum(x => x.TotalMoney);

                var totalsale = new TotalSale();
                totalsale.thang = i;
                if (model != null)
                    totalsale.tong = model;
                else
                    totalsale.tong = 0;
                lstTotal.Add(totalsale);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CateSale()
        {
            var lstTotal = new List<TotalSale>();
            foreach (var item in db.Categories.ToList())
            {
                var totalsale = new TotalSale();
                decimal? tong = 0;

                var model = (from or in db.Orders
                             join detail in db.Order_Detail on or.ID equals detail.Order_ID
                             join pro in db.Products on detail.Product_ID equals pro.ID
                             where pro.Category_ID == item.ID && or.Payment == 1 || pro.Category_ID == item.ID && or.Status == 3
                             select new TotalSale
                             {
                                 tong = or.TotalMoney
                             }).Sum(x => x.tong);


                if (model != null)
                    tong += model;
                else
                    tong += 0;
                totalsale.tong = tong;
                totalsale.CategoryName = item.Name;
                lstTotal.Add(totalsale);
            }
            return Json(lstTotal, JsonRequestBehavior.AllowGet);

        }

      
    }
}