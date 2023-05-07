using Newtonsoft.Json;
using Watch.Models.DTO;
using Watch.Models.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;

namespace Watch.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Admin/Product
        [CustomRoleProvider(RoleName = new string[] { "Staff", "Boss" })]
        public ActionResult Index()
        {
                var query = db.Products.Where(p=>p.Status != -1).ToList();
                ViewBag.lstCategory = db.Categories.ToList();
                return View(query.OrderByDescending(x => x.ID).ToList());
        }

        [CustomRoleProvider(RoleName = new string[] { "Staff", "Boss" })]
        // GET: Admin/Product/Create
        public ActionResult Add()
        {
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstBrand = db.Brands.ToList();
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmAdd(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                long maxid = (db.Products.Count() > 0 ? db.Products.Max(x => x.ID) : 0);
                entity.Metatitle = Str_Metatitle(entity.Product_Name);
                entity.Status = 1;
               
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/Client/img/product"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName +DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/Client/img/product/"), filename);
                    Image.SaveAs(path);
                    entity.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    entity.Image = Image.FileName;
                }

                db.Products.Add(entity);
                db.SaveChanges();
                TempData["message"] = "Thêm sản phẩm thành công.";
                TempData["alert"] = "alert-success";

                return RedirectToAction("Index");
                
            }
            catch
            {
                TempData["message"] = "Thêm sản phẩm không thành công.";
                TempData["alert"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Product/Edit/5
        [CustomRoleProvider(RoleName = new string[] { "Staff", "Boss" })]
        public ActionResult Edit(long ID)
        {
            ViewBag.product = db.Products.Find(ID);
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstBrand = db.Brands.ToList();
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult frmEdit(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                var pro = db.Products.Find(entity.ID);
                pro.Product_Name = entity.Product_Name;
                pro.Metatitle = Str_Metatitle(entity.Product_Name);
                pro.Promotion_Price = entity.Promotion_Price;
                pro.Price = entity.Price;
                pro.Category_ID = entity.Category_ID;
                pro.Desription = entity.Desription;
                pro.Configuration = entity.Configuration;
                pro.Quantity = entity.Quantity;
                try
                {
                    if (Image != null && pro.Image != Image.FileName)
                    {
                        //Xóa file cũ
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product"), pro.Image));
                        //Thêm hình ảnh
                        var path = Path.Combine(Server.MapPath("~/Assets/Client/img/product"), Image.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            string extensionName = Path.GetExtension(Image.FileName);
                            string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                            path = Path.Combine(Server.MapPath("~/Assets/Client/img/product/"), filename);
                            Image.SaveAs(path);
                            pro.Image = filename;
                        }
                        else
                        {
                            Image.SaveAs(path);
                            pro.Image = Image.FileName;
                        }
                    }
                    db.SaveChanges();
                    TempData["message"] = "Cập nhật sản phẩm thành công.";
                    TempData["alert"] = "alert-success";
                    return RedirectToAction("Index");
                    
                }
                catch
                {
                    db.SaveChanges();
                    TempData["message"] = "Cập nhật sản phẩm không thành công.";
                    TempData["alert"] = "alert-danger";
                    return RedirectToAction("Index");
                }

                
            }
            catch
            {
                TempData["message"] = "Cập nhật sản phẩm không thành công.";
                TempData["alert"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        // GET: Admin/Product/Delete/5
        public JsonResult Delete(long ID)
        {
            //try
            //{
            //    var product = db.Products.Find(ID);
            //    var lstImage = db.Images.Where(x => x.Product_ID == ID).ToList();
            //    foreach (var item in lstImage)
            //    {
            //        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product-detail"), item.Image1));
            //        db.Images.Remove(item);
            //    }
            //    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product"), product.Image));

            //    db.Products.Remove(product);
            //    db.SaveChanges();
            //    return Json(new
            //    {
            //        status = true
            //    });
            //}
            //catch
            //{
            //    return Json(new
            //    {
            //        status = false
            //    });
            //}
            try
            {
                var product = db.Products.Find(ID);
                var orderDetail = db.Order_Detail.Where(o => o.Product_ID == ID);
                if (orderDetail != null)
                {
                    //product.Status = -1;
                    //db.SaveChanges();
                    return Json(new
                    {
                        status = false
                    });
                }
                else
                {
                    var lstImage = db.Images.Where(x => x.Product_ID == ID).ToList();
                    foreach (var item in lstImage)
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product-detail"), item.Image1));
                        db.Images.Remove(item);
                    }
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product"), product.Image));

                    db.Products.Remove(product);
                    db.SaveChanges();
                    return Json(new
                    {
                        status = true
                    });
                }
               
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }

        public JsonResult changeStatus(long ID)
        {
            var pro = db.Products.Find(ID);
            if (pro.Status == 1)
                pro.Status = 0;
            else
                pro.Status = 1;
            db.SaveChanges();
            return Json(new
            {
                status = 1
            });
        }


        public ActionResult Images(long ID)
        {
            ViewBag.lstImage = db.Images.Where(x => x.Product_ID == ID).OrderBy(x => x.ID).ToList();
            ViewBag.Product = db.Products.Find(ID);
            return View();
        }


        [HttpPost]
        public ActionResult Upload_Mutil_Image(long Product_ID, HttpPostedFileBase[] images)
        {
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in images)
                {
                    var imgs = new Image();
                    imgs.Product_ID = Product_ID;
                    var path = Path.Combine(Server.MapPath("~/Assets/Client/img/product-detail"), file.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        string extensionName = Path.GetExtension(file.FileName);
                        string filename = file.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                        path = Path.Combine(Server.MapPath("~/Assets/Client/img/product-detail"), filename);
                        file.SaveAs(path);
                        imgs.Image1 = filename;
                        db.Images.Add(imgs);
                        db.SaveChanges();

                    }
                    else
                    {
                        file.SaveAs(path);
                        imgs.Image1 = file.FileName;
                        db.Images.Add(imgs);
                        db.SaveChanges();
                    }

                }
            }
            TempData["message"] = "Thêm hình ảnh thành công.";
            TempData["alert"] = "alert-success";
            return RedirectToAction("Images", new { ID = Product_ID });
        }

        public JsonResult Del_Images(long ID)
        {

            var img = db.Images.Find(ID);
            if(img.Image1 != null)
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/Client/img/product-detail"), img.Image1));
            db.Images.Remove(img);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });

        }

        //Chuyển tên sản phẩm thành metatitle
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
                {
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]).Replace("“", string.Empty).Replace("”", string.Empty);
                    str = str.Replace("\"", string.Empty).Replace("'", string.Empty).Replace("`", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);
                    str = str.Replace(".", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty).Replace(":", string.Empty);
                    str = str.Replace("?", string.Empty).Replace("+", string.Empty);
                }
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
        //list các bài đánh giá
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        public ActionResult review()
        {
            var listReview = db.Reviews.ToList();
            return View(listReview);
        }
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        //phản hồi 
        public ActionResult feedback(long ID)
        {
            return View();
        }
        [CustomRoleProvider(RoleName = new string[] { "Editor", "Boss" })]
        [HttpPost]
        public ActionResult HandleFeedback(Review entity)
        {
            try
            {
                var r = db.Reviews.Find(entity.ID);
                r.Feedback = entity.Feedback;
                r.FeedbackDate = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "Phản hồi thành công";
                TempData["alert"] = "alert-success";
                return Redirect("/admin/product/review");
            }
            catch
            {
                TempData["message"] = "Phản hồi KHÔNG thành công";
                TempData["alert"] = "alert-danger";
                return Redirect("/admin/product/review");
            }
        }

        public JsonResult GetByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var review = db.Reviews.Find(ID);
            return Json(review, JsonRequestBehavior.AllowGet);
        }

    }
}
