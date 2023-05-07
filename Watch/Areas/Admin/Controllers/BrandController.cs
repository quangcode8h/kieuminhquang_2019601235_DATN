using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;
using Watch.Models.EF;

namespace Watch.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Brand
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        public ActionResult Index()
        {
            var model = db.Brands.OrderBy(x => x.ID).ToList();
            return View(model);
        }

        public JsonResult Delete(long ID)
        {

            try
            {
                var product = db.Products.Where(p=> p.Brand_ID == ID).FirstOrDefault();
                if (product != null)
                    return Json(new
                    {
                        status = false
                    });
                var model = db.Brands.Find(ID);
                //Xóa file cũ
                if (model.Image != null)
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), model.Image));
                db.Brands.Remove(model);
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
        public ActionResult add(Brand entity, HttpPostedFileBase Image)
        {
            try
            {
                entity.Metatitle = Str_Metatitle(entity.Name.Trim());
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), filename);
                    Image.SaveAs(path);
                    entity.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.Image = Image.FileName;
                }
                db.Brands.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm thương hiệu thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/brand");

            }
            catch
            {
                TempData["message"] = "Thêm thương hiệu KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/brand");
            }
        }

        [HttpPost]
        public ActionResult edit(Brand entity, HttpPostedFileBase Image)
        {
            try
            {
                var prv = db.Brands.Find(entity.ID);
                prv.Name = entity.Name.Trim();
                prv.Metatitle = Str_Metatitle(entity.Name.Trim());

                if (Image != null && prv.Image != Image.FileName)
                {
                    //Xóa file cũ
                    if (prv.Image != null)
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), prv.Image));
                    //Thêm hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), Image.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(Image.FileName);
                        string filename = Image.FileName + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/brand"), filename);
                        Image.SaveAs(path);
                        prv.Image = filename;
                    }
                    else
                    {
                        Image.SaveAs(path);
                        prv.Image = Image.FileName;
                    }
                }

                db.SaveChanges();
                TempData["message"] = "Cập nhật thương hiệu thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/brand");
            }
            catch
            {
                TempData["message"] = "Cập nhật thương hiệu KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/brand");
            }
        }

        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var brand = db.Brands.Find(ID);
            return Json(brand, JsonRequestBehavior.AllowGet);
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