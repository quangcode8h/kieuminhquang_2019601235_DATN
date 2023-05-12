using Watch.Models.DTO;
using Watch.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Watch.Models.Business;
using System.Security.Cryptography;

namespace Watch.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        HandleMd5 handleMd5 = new HandleMd5();
        // GET: Admin/Login
        //[CustomRoleProvider(Type = 1)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomRoleProvider(RoleName = new string[] { "Admin", "Boss" })]
        public ActionResult List()
        {
            var model = db.Managers.Where(x => x.Account != "admin" && x.Account != "boss").OrderBy(x => x.ID).ToList();
            ViewBag.lstPhanQuyen = db.Roles.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult frmLogin(User model)
        {
            var admin = db.Managers.SingleOrDefault(a => a.Account == model.Account);
            int checkPassword = (admin != null) ? string.Compare(handleMd5.DecryptString(admin.Password.Trim()), model.Password.Trim()) : -1;
            if (admin != null && checkPassword == 0)
            {
                if(admin.Status == false)
                {
                    TempData["error"] = "Tài khoản đăng nhập đã bị khóa.";
                    return Redirect("/admin/login");
                }
                else
                {
                    Update_LastLogin(admin.ID);
                    Session["admin"] = admin;
                    return Redirect("/admin/home");
                }
                
            }
            else
            {
                TempData["error"] = "Tài khoản hoặc mật khẩu không chính xác";
                return Redirect("/admin/login");
            }
        }

        public ActionResult Logout()
        {
            Session["admin"] = null;
            return Redirect("/admin/login");
        }

        //Cập nhật trạng thái
        public JsonResult changeStatus(long ID)
        {
            var user = db.Managers.Find(ID);
            if (user.Status == true)
                user.Status = false;
            else
                user.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        //Cập nhật phân quyền
        public ActionResult UpdateRole(long RoleID, long AdminID)
        {
            var admin = db.Managers.SingleOrDefault(x => x.ID == AdminID);
            if(admin!= null)
            {
                admin.RoleID = RoleID;
            }
            db.SaveChanges();
            return RedirectToAction("List");
        }

        public ActionResult addAdmin(Manager entity)
        {
            var ad = db.Managers.Where(a => a.Account == entity.Account).FirstOrDefault();
            if(ad != null)
            {
                TempData["error"] = "Tài khoản đã tồn tại";
            }
            else
            {
                entity.Status = true;
                entity.Password = handleMd5.EncryptString(entity.Password);
                entity.RoleID = 6;
                entity.Image = "default.png";
                db.Managers.Add(entity);
                db.SaveChanges();
                TempData["success"] = "Tạo tài khoản thành công";
            }
            return RedirectToAction("List");
        }

        //Xóa tài khoản
        public ActionResult deleteAdmin(long ID)
        {
            var ad = db.Managers.Where(a => a.ID == ID).FirstOrDefault();
            if(ad.RoleID == 6)
            {
                db.Managers.Remove(ad);
                db.SaveChanges();
                TempData["success"] = "Xóa tài khoản thành công";
                return RedirectToAction("List");
            }
            TempData["error"] = "Xóa tài khoản KHÔNG thành công";
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult frmAdd(Watch.Models.EF.Manager entity, HttpPostedFileBase Image)
        {
            try
            {
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/admin"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/admin"), filename);
                    Image.SaveAs(path);
                    entity.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.Image = Image.FileName;
                }
                //entity.Password = new MD5().Encrypt_MD5(entity.Password);
                entity.Status = true;
                db.Managers.Add(entity);
                db.SaveChanges();
                TempData["massage"] = "Thêm truy cập thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/login/list");

            }
            catch
            {
                TempData["massage"] = "Thêm truy cập KHÔNG thành công";
                TempData["alert"] = "alert-danger";

                return Redirect("/admin/login/list");
            }
        }

        public ActionResult Update()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmUpdate(Watch.Models.EF.Manager entity, HttpPostedFileBase Image)
        {
            try
            {
                var prv = db.Managers.Find(entity.ID);
                prv.Fullname = entity.Fullname;

                if (Image != null && prv.Image != Image.FileName)
                {
                    //Xóa file cũ
                    if (prv.Image != null)
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/admin"), prv.Image));
                    //Thêm hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/admin"), Image.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(Image.FileName);
                        string filename = Image.FileName + DateTime.Now.ToString("hhmmssddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/admin"), filename);
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

                Session["admin"] = db.Managers.Find(entity.ID);
                TempData["message"] = "Cập nhật thông tin thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/login/update");
            }
            catch
            {
                TempData["message"] = "Cập nhật thông tin KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/login/update");
            }
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmchangePass(string Old_Pass, string New_Pass)
        {
            var admin = Session["admin"] as Watch.Models.EF.Manager;
            var user = db.Managers.Find(admin.ID);
            if (handleMd5.DecryptString(user.Password.Trim()) == Old_Pass.Trim())
            {
                user.Password = handleMd5.EncryptString(New_Pass.Trim());
                db.SaveChanges();
                Session["admin"] = null;
                TempData["error"] = "Bạn vui lòng đăng nhập lại sau khi đổi mật khẩu.";
                return Redirect("/admin/login");
            }
            else
            {
                TempData["error"] = "Mật khẩu cũ không đúng, vui lòng nhập lại.";
                return Redirect("/admin/login/changePass");
            }

        }

        public void Update_LastLogin(long admin_id)
        {
            var admin = db.Managers.Find(admin_id);
            admin.LastLogin = DateTime.Now;
            db.SaveChanges();
        }
    }
}