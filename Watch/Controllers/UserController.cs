using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.EF;
using Watch.Models.Business;
using PagedList;
namespace Watch.Controllers
{
    public class UserController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: User
        HandleMd5 handleMd5 = new HandleMd5();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmLogin(User model)
        {
            var user = db.Users.SingleOrDefault(u=> u.Account == model.Account);
            var checkPassword = (user != null) ? string.Compare(handleMd5.DecryptString(user.Password.Trim()), model.Password.Trim()) : -1;
            if (user != null && checkPassword == 0)
            {
                if (user.Status == false)
                {
                    TempData["error"] = "Tài khoản của bạn đã bị khóa.";
                    return Redirect("/user/login");
                }
                else
                {
                    Session["user"] = user;
                    if (Session["check_payment"] != null)
                    {
                        return Redirect("/cart/payment");
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }

            }
            else
            {
                TempData["error"] = "Tài khoản hoặc mật khẩu không chính xác.";
                return Redirect("/user/login");
            }
        }



        public ActionResult Register()
        {
            return View();
        }

        public ActionResult frmRegister(User entity)
        {
            if (db.Users.Count(x => x.Account == entity.Account) > 0)
            {
                TempData["error"] = "Tài khoản đã tồn tại hoặc đã có lỗi khi đăng ký, vui lòng thử lại sau.";
                return Redirect("/user/register");
            }
            else
            {
                entity.Status = true;
                entity.Password = handleMd5.EncryptString(entity.Password);
                db.Users.Add(entity);
                db.SaveChanges();
                TempData["success"] = "Đăng ký thành công. Vui lòng đăng nhập lại để tiếp tục.";
                return Redirect("/user/login");
            }
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmChangePass(string Ex_password, string New_Password)
        {
            var user = Session["user"] as User;
            if (handleMd5.DecryptString(user.Password.Trim()) == Ex_password.Trim())
            {
                var entity = db.Users.Find(user.ID);
                entity.Password = handleMd5.EncryptString(New_Password);
                db.SaveChanges();

                TempData["error"] = "Bạn vui lòng đăng nhập lại sau khi đổi mật khẩu.";
                Session["user"] = null;
                return Redirect("/user/login");
            }
            else
            {
                TempData["error"] = "Mật khẩu cũ không trùng, vui lòng nhập lại.";
                return Redirect("/user/changepass");
            }

        }

        public ActionResult logout()
        {
            Session["user"] = null;
            return Redirect("/user/login");
        }

        public ActionResult order(long ID, int ?page)
        {
            var lisrOrder = (from o in db.Orders where o.User_ID == ID select o).OrderByDescending(x=> x.CreatedDate).ToList();
            var listProduct = (from p in db.Products join od in db.Order_Detail on p.ID equals od.Product_ID 
                               join o in db.Orders on od.Order_ID equals o.ID where o.User_ID == ID select new
                               {
                                   p.ID, p.Product_Name, p.Promotion_Price, p.Image, od.Order_ID, od.Quantity
                               }).ToList();
            if (lisrOrder == null) page = 1;
            List<ProductInOrder> listProductInOrder = new List<ProductInOrder>();
            foreach(var o in lisrOrder)
            {
                foreach(var p in listProduct)
                {
                    if(p.Order_ID == o.ID)
                    {
                        int check = 0;
                        foreach(var po in listProductInOrder)
                        {
                            if(po.id == p.ID && po.OrderID == p.Order_ID)
                            {
                                po.amount = po.amount + int.Parse(p.Quantity.ToString()) ;
                                check = 1;
                                break;
                            }
                        }
                        if(check == 0)
                        {
                            ProductInOrder pro = new ProductInOrder();
                            pro.id = p.ID;
                            pro.ProductName = p.Product_Name;
                            pro.Price = double.Parse(p.Promotion_Price.ToString());
                            pro.Image = p.Image;
                            pro.OrderID = long.Parse(p.Order_ID.ToString());
                            pro.amount = int.Parse(p.Quantity.ToString());
                            listProductInOrder.Add(pro);
                        }
                    }
                }
            }
            ViewBag.listProductInOrder = listProductInOrder;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.ID = ID;
            return View(lisrOrder.ToPagedList(pageNumber, pageSize));
        }
        public JsonResult CancelOrder(long ID)
        {
            var order = db.Orders.Find(ID);
            if (order.Status == 1)
            {
                order.Status = 0;
                order.CancerDate = DateTime.Now;
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
                return Json(new
                {
                    status = false
                });
        }
    }
}