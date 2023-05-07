
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
    public class UserController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/User
        [CustomRoleProvider(RoleName = new string[] { "Boss", "Admin" })]
        public ActionResult Index()
        {
            var lstUser = db.Users.ToList();
            return View(lstUser);
        }


        //Xóa tài khoản
        public JsonResult Delete(long ID)
        {
            var user = db.Users.Find(ID);
            db.Users.Remove(user);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult changeStatus(long ID)
        {
            var user = db.Users.Find(ID);
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
        
    }
}