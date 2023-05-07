using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;
using Watch.Models.DTO;
using Watch.Models.EF;

namespace Watch.Areas.Admin.Controllers
{
    public class WarrantyController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Warranty
        [CustomRoleProvider(RoleName = new string[] {  "Boss", "Staff" })]
        public ActionResult Index()
        {
            var model = db.Warranties.ToList();

            var lstProduct = (from detail in db.Order_Detail
                              join order in db.Orders on detail.Order_ID equals order.ID
                              join pro in db.Products on detail.Product_ID equals pro.ID
                              where order.Status == 3 || order.Payment == 1
                              select new Order_DetailDTO()
                              {
                                  Product_ID = pro.ID,
                                  Product_Name = pro.Product_Name
                              }).Distinct();
            ViewBag.lstProduct = lstProduct.ToList();
            return View(model);
        }


        public JsonResult Delete(long ID)
        {

            try
            {
                var warranty = db.Warranties.Find(ID);

                db.Warranties.Remove(warranty);
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




        [HttpPost]
        public ActionResult frmAdd(Warranty entity)
        {
            var admin = Session["admin"] as Watch.Models.EF.Manager;
            try
            {
                entity.Admin_ID = admin.ID;
                entity.CreatedDate = DateTime.Now;
                //entity.Date_Start = DateTime.Parse(entity.Date_Start.ToString());
                //entity.Date_End = DateTime.Parse(entity.Date_End.ToString());
                db.Warranties.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm thông tin bảo hành thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/warranty");

            }
            catch
            {
                TempData["message"] = "Thêm thông tin bảo hành KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/warranty");
            }
        }

        [HttpPost]
        public ActionResult frmEdit(Warranty entity)
        {
            try
            {
                var prv = db.Warranties.Find(entity.ID);
                prv.CustomerName = entity.CustomerName;
                prv.CustomerPhone = entity.CustomerPhone;
                prv.Email = entity.Email;
                prv.Product_ID = entity.Product_ID;
                prv.Date_Start = entity.Date_Start;
                prv.Date_End = entity.Date_End;
                prv.Description = entity.Description;
                db.SaveChanges();
                TempData["message"] = "Cập nhật thông tin bảo hành thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/warranty");
            }
            catch
            {
                TempData["message"] = "Cập nhật thông tin bảo hành KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/warranty");
            }
        }

        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var warranty = db.Warranties.Find(ID);
            var model = new WarrantyDTO();
            model.ID = warranty.ID;
            model.CustomerName = warranty.CustomerName;
            model.CustomerPhone = warranty.CustomerPhone;
            model.Description = warranty.Description;
            model.Product_ID = warranty.Product_ID;
            model.Date_End = warranty.Date_End.Value.ToString("dd-MM-yyyy");
            model.Date_Start = warranty.Date_Start.Value.ToString("dd-MM-yyyy");
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}