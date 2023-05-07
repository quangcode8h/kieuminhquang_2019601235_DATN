
using Watch.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Watch.Models.Business;

namespace Watch.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Admin/Category
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Provider
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        public ActionResult Index()
        {
            var model = db.Categories.OrderByDescending(x => x.ID).ToList();
            return View(model);
        }


        public JsonResult Delete(long ID)
        {

            try
            {
                var category = db.Categories.Find(ID);
                
                db.Categories.Remove(category);
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
        public ActionResult frmAdd(Category entity)
        {
            try
            {
                entity.Metatitle = Str_Metatitle(entity.Name.Trim());
                db.Categories.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm danh mục sản phẩm thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/category");

            }
            catch
            {
                TempData["message"] = "Thêm danh mục sản phẩm KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/category");
            }
        }

        [HttpPost]
        public ActionResult frmEdit(Category entity)
        {
            try
            {
                var prv = db.Categories.Find(entity.ID);
                prv.Name = entity.Name.Trim();
                prv.Metatitle = Str_Metatitle(entity.Name.Trim());

                db.SaveChanges();
                TempData["message"] = "Cập nhật danh mục sản phẩm thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/category");
            }
            catch
            {
                TempData["message"] = "Cập nhật danh mục sản phẩm KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/category");
            }
        }

        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var category = db.Categories.Find(ID);
            return Json(category, JsonRequestBehavior.AllowGet);
        }


        public string Str_Metatitle(string str)
        {
            string[] VietNamChar = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ:/"
            };
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + "-";
            }
            return meta;
        }
    }
}