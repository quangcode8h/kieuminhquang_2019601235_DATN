using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Watch.Models.Business;
using Watch.Models.DTO;
using Watch.Models.EF;

namespace Watch.Controllers
{
    public class ProductController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();
        // GET: Product
        public ActionResult Index(string Metatitle, long ID)
        {
            var product = db.Products.Find(ID);
            ViewBag.product = product;
            ViewBag.lstSameBrand = db.Products.Where(x => x.Brand_ID == product.Brand_ID && x.ID != ID && x.Status == 1).Take(4).ToList();
            ViewBag.lstSameCategory = db.Products.Where(x => x.Category_ID == product.Category_ID && x.ID != ID && x.Status == 1).Take(5).ToList();
            ViewBag.lstReview = db.Reviews.Where(x => x.Product_ID == ID).OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.lstImage = db.Images.Where(x => x.Product_ID == ID).ToList();
            ViewBag.lstBrand = db.Brands.ToList();
            ViewBag.lstCategory = db.Categories.ToList();
            return View();
        }


        public JsonResult addReview(string json_review)
        {
            var JsonReview = new JavaScriptSerializer().Deserialize<List<ReviewDTO>>(json_review);
            var review = new Review();
            foreach (var item in JsonReview)
            {
                review.Content = item.Content;
                review.CreatedDate = DateTime.Now;
                review.User_ID = item.User_ID;
                review.Product_ID = item.Product_ID;
                review.Status = true;

            }
            var orderDetailCheck = (from o in db.Orders join od in db.Order_Detail on o.ID equals od.Order_ID where o.User_ID == review.User_ID && od.Product_ID == review.Product_ID && o.Status == 3 select od).FirstOrDefault();
            
            if (orderDetailCheck != null)
            {
                var res = new ProductBusiness().addReview(review);
                if (res)
                {
                    return Json(new
                    {
                        status = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false
                    });
                }
            }
            return Json(new
            {
                status = false
            });

        }


        public JsonResult ListName(string q)
        {
            var query = db.Products.Where(x => x.Status == 1 && x.Product_Name.Contains(q)).Select(x => new { x.Product_Name, x.Image });
            return Json(new
            {
                data = query,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword, string type = null, string order = null, int page = 1, int pagesize = 12)
        {
            string[] key = keyword.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var product_name = new List<Product>();//Tìm theo tên sản phẩm
            ViewBag.KeyWord = keyword;
            product_name = db.Products.Where(p => p.Product_Name.Contains(keyword) && p.Status == 1).ToList();
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstBrand = db.Brands.ToList();
            ViewBag.ProductCount = product_name.Count;
            return View(product_name.ToPagedList(page, pagesize));

        }
    }
}