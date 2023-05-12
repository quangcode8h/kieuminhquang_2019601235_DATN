using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models.Business;
using Watch.Models.EF;

namespace Watch.Controllers
{
    public class HomeController : Controller
    {
        private WatchDBEntities db = new WatchDBEntities();

        public ActionResult Index()
        {
            ViewBag.lstSlide = db.slides.ToList();
            ViewBag.lstProduct = db.Products.ToList();
            ViewBag.lstBrand = new ProductBusiness().getRandomBrand();
            ViewBag.NewProduct = db.Products.Where(x => x.Status == 1).OrderByDescending(x => x.ID).ToList();
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstFeatureProduct = db.Products.OrderByDescending(p => p.Price - p.Promotion_Price).ToList();
            return View();
        }
        public ActionResult lstProBy_Category(string Metatitle, long ID, string type = null, string order = null, int page = 1, int pagesize = 12)
        {
            ViewBag.Category = new ProductBusiness().FindCategory(ID);
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstBrand = db.Brands.ToList();

            if (type != null && order != null)
            {
                var product = new List<Product>();
                if (type == "name")
                {
                    if (order == "a-to-z")
                    {
                        product = db.Products.Where(x => x.Category_ID == ID && x.Status == 1).OrderBy(x => x.Product_Name).ToList();
                    }
                    else
                    {
                        product = db.Products.Where(x => x.Category_ID == ID && x.Status == 1).OrderByDescending(x => x.Product_Name).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        product = db.Products.Where(x => x.Category_ID == ID && x.Status == 1).OrderByDescending(x => x.Promotion_Price).ToList();
                    }
                    else
                    {
                        product = db.Products.Where(x => x.Category_ID == ID && x.Status == 1).OrderBy(x => x.Promotion_Price).ToList();
                    }
                }

                ViewBag.Type = type;
                ViewBag.Order = order;
                return View(product.ToPagedList(page, pagesize));
            }
            else
            {
                var model = db.Products.Where(x => x.Category_ID == ID && x.Status == 1).OrderBy(x => x.ID).ToPagedList(page, pagesize);
                return View(model);
            }

        }


        public ActionResult lstProBy_Brand(string Metatitle, long ID, string type = null, string order = null, int page = 1, int pagesize = 12)
        {
            ViewBag.Brand = db.Brands.Find(ID);
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstBrand = db.Brands.ToList();
            if (type != null && order != null)
            {
                var product = new List<Product>();
                if (type == "name")
                {
                    if (order == "a-to-z")
                    {
                        product = db.Products.Where(x => x.Brand_ID == ID && x.Status == 1).OrderBy(x => x.Product_Name).ToList();
                    }
                    else
                    {
                        product = db.Products.Where(x => x.Brand_ID == ID && x.Status == 1).OrderByDescending(x => x.Product_Name).ToList();
                    }
                }
                else
                {
                    if (order == "desc")
                    {
                        product = db.Products.Where(x => x.Brand_ID == ID && x.Status == 1).OrderByDescending(x => x.Promotion_Price).ToList();
                    }
                    else
                    {
                        product = db.Products.Where(x => x.Brand_ID == ID && x.Status == 1).OrderBy(x => x.Promotion_Price).ToList();
                    }
                }

                ViewBag.Type = type;
                ViewBag.Order = order;
                return View(product.ToPagedList(page, pagesize));
            }
            else
            {
                var model = db.Products.Where(x => x.Brand_ID == ID && x.Status == 1).OrderBy(x => x.ID).ToPagedList(page, pagesize);
                return View(model);
            }
        }

        [ChildActionOnly]
        public PartialViewResult Category()
        {
            ViewBag.lstCategory = db.Categories.ToList();
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult Brand(string type)
        {
            ViewBag.lstBrand = db.Brands.ToList();
            ViewBag.Type = type;
            return PartialView();
        }
    }
}