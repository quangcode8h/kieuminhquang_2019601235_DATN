using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.EF;

namespace Watch.Models.Business
{
    public class CustomRoleProvider : AuthorizeAttribute
    {
        WatchDBEntities db = new WatchDBEntities();

        public string[] RoleName { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool tg = false;
            var admin = HttpContext.Current.Session["admin"] as Manager;
            try
            {
                foreach (var item in RoleName)
                {
                    var role = db.Roles.Find(admin.RoleID);
                    if (role.RoleName == item)
                    {
                        tg = true;
                        break;
                    }
                    else
                        tg = false;
                }
                if (tg)
                    return true;
                else
                    return false;

            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult()
            {
                ViewName = "/Areas/Admin/Views/Shared/Error.cshtml"
            };
        }
    }
}