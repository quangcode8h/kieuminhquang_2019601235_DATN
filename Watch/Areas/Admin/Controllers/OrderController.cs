using Watch.Models.DTO;
using Watch.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;

namespace Watch.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();

        //Đơn đã thanh toán
        [CustomRoleProvider(RoleName = new string[] { "Staff", "Boss" })]
        public ActionResult Index()
        {
            var model = db.Orders.OrderByDescending(x => x.CreatedDate).ToList();
            return View(model);
        }

        [CustomRoleProvider(RoleName = new string[] { "Staff", "Boss" })]
        public ActionResult Order_Detail(long ID)
        {
            var query = db.Order_Detail.Where(x => x.Order_ID == ID).OrderByDescending(x => x.ID).ToList();
            ViewBag.order = db.Orders.Find(ID);
            return View(query);
        }

        //kích hoạt đã thanh toán
        public JsonResult changeStatus(long ID)
        {
            var order = db.Orders.Find(ID);
            if (order.Status == 1)
            {
                order.Status = 2;
                order.ShipDate = DateTime.Now;
            }
            else if (order.Status == 2)
            {
                order.Status = 3;
                order.PaidDate = DateTime.Now;

                foreach (var item in db.Order_Detail.Where(x => x.Order_ID == ID))
                {
                    new ProductBusiness().Subtract_Quantity((long)item.Product_ID, (int)item.Quantity);
                }
            }
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }


        //Từ chối đơn hàng
        public JsonResult Delete(long ID)
        {
            try
            {
                var order = db.Orders.Find(ID);
                order.Status = -1;
                order.CancerDate = DateTime.Now;
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }

    }
}