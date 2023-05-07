using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.EF;
using Watch.Models.Business;
using System.IO;
namespace Watch.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Slide
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        public ActionResult Index()
        {
            var slides = db.slides.ToList();
            return View(slides);
        }


        [HttpPost]
        public ActionResult add(slide entity, HttpPostedFileBase Image)
        {
            try
            {
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/slider"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/slider"), filename);
                    Image.SaveAs(path);
                    entity.img = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.img = Image.FileName;
                }
                db.slides.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm slide thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/slide");

            }
            catch
            {
                TempData["message"] = "Thêm slide KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/slide");
            }
        }
        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var slide = db.slides.Find(ID);
            return Json(slide, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(long ID)
        {
            try
            {
                var model = db.slides.Find(ID);
                //Xóa file cũ
                if (model.img != null)
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/slider"), model.img));
                db.slides.Remove(model);
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