
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Watch.Models.DTO;
using Watch.Models.EF;

namespace Watch.Models.Business
{
    public class ProductBusiness
    {
        private WatchDBEntities db = new WatchDBEntities();

        public Product findID(long ID)
        {
            return db.Products.Find(ID);
        }

        //Tìm kiếm sản phẩm theo tên sp
        public Product searchProduct(string product_name)
        {
            return db.Products.Single(x => x.Product_Name == product_name);
        }
        
        //Lấy chi tiết hình ảnh sản phẩm
        public List<Image> getLstImage(long ID)
        {
            return db.Images.Where(x => x.Product_ID == ID).ToList();
        }

        //Lấy điểm đánh giá trung bình
        //public List<ProductDTO> getRateAverage()
        //{
        //    var lstReview = db.Reviews.ToList();
        //    var lstProduct = new List<ProductDTO>();

        //    int? SumRate = 0;

        //    int count = 0;
        //    foreach (var item in lstReview)
        //    {
        //        var pro = new ProductDTO();
        //        pro.ID = (long)item.Product_ID;
        //        pro.CountReview = db.Reviews.Count(x => x.Product_ID == item.Product_ID);
        //        count = db.Reviews.Count(x => x.Product_ID == item.Product_ID);
        //        SumRate = db.Reviews.Where(x => x.Product_ID == item.Product_ID).Sum(x => x.Rating);
        //        pro.Rate_Average = System.Math.Round((double)SumRate / (double)count, 2);
        //        lstProduct.Add(pro);
        //    }

        //    return lstProduct;
        //}


        //Cộng tồn kho
        public void AddQuantity(long ID, int quantity)
        {
            var product = db.Products.Find(ID);
            product.Quantity += quantity;
            db.SaveChanges();
        }

        //Trừ tồn kho sau khi thanh toán đơn hàng
        public void Subtract_Quantity(long ID, int quantity)
        {
            var product = db.Products.Find(ID);
            product.Quantity -= quantity;
            db.SaveChanges();
        }

        //Lấy ngẫu nhiên sản phẩm
        public List<Product> getRandomProduct()
        {
            var lst_pro = new List<Product>();
            var random = new Random();
            long index = random.Next(1, 4);
            if (index == 1)
                lst_pro = db.Products.Where(x => x.Status == 1).OrderByDescending(x => x.Product_Name).ToList();
            else if(index == 2)
                lst_pro = db.Products.Where(x => x.Status == 1).OrderBy(x => x.Product_Name).ToList();
            else if (index == 3)
                lst_pro = db.Products.Where(x => x.Status == 1).OrderByDescending(x => x.ID).ToList();
            else
                lst_pro = db.Products.Where(x => x.Status == 1).OrderBy(x => x.ID).ToList();

            return lst_pro.Where(x => x.Status == 1).ToList();   
        }

        //Lấy ngẫu nhiên thương hiệu
        public List<Brand> getRandomBrand()
        {
            var lst_brand = new List<Brand>();
            var random = new Random();
            long index = random.Next(1, 4);
            if (index == 1)
                lst_brand = db.Brands.OrderByDescending(x => x.Name).Take(2).ToList();
            else if (index == 2)
                lst_brand = db.Brands.OrderBy(x => x.Name).Take(2).ToList();
            else if (index == 3)
                lst_brand = db.Brands.OrderByDescending(x => x.ID).Take(2).ToList();
            else
                lst_brand = db.Brands.OrderBy(x => x.ID).Take(2).ToList();

            return lst_brand.Where(x => x.Status == true).ToList();
        }

        //Lấy toàn bộ sản phẩm
        public List<Product> getAllProduct()
        {
            return db.Products.ToList();
        }
        

        //Lấy sản phẩm bán chạy
        public List<Order_DetailDTO> GetBestSellerProduct()
        {
            var listProduct_sell = new List<Order_DetailDTO>();
            foreach (var item in db.Order_Detail.Where(x => x.Order.Payment == 1 || x.Order.Status == 3).ToList())
            {
                if (listProduct_sell.Exists(x => x.Product_ID == item.Product_ID))
                {
                    foreach (var jtem in listProduct_sell)
                    {
                        if (jtem.Product_ID == item.Product_ID)
                        {
                            jtem.Quantity += item.Quantity;
                        }
                    }

                }
                else
                {
                    var productsell = new Order_DetailDTO();
                    productsell.Product = item.Product;
                    productsell.Quantity = item.Quantity;
                    listProduct_sell.Add(productsell);
                }
            }

            return listProduct_sell.OrderByDescending(x => x.Quantity).ToList();
        }

        //Lấy danh mục sản phẩm
        public List<Category> GetCategories()
        {
            return db.Categories.ToList();
        }

        //TÌm kiếm danh mục sp
        public Category FindCategory(long ID)
        {
            return db.Categories.Find(ID);
        }


        //Lấy danh mục sp ngẫu nhiên
        //public List<ProductDTO> getParentCategory()
        //{
        //    var lst_id = db.ParentCategories.Select(x => x.ID).ToArray();
        //    var lst_pro = new List<ProductDTO>();
        //    var lst_parent = new List<long>();
        //    int dem = 0;

        //    var random = new Random();
        //    while (true)
        //    {
        //        if (dem == 2)
        //            break;
        //        long index = random.Next(lst_id.Length - 1);
        //        var parent = new ProductDTO();
        //        var parent_id = lst_id[index];
        //        parent.ParentCategorys = db.ParentCategories.Find(parent_id);
        //        parent.LstProducts = db.Products.Where(x => x.Category.ParentCategory_ID == parent_id).ToList();
        //        //parent.ParentCategory_ID = lst_id[index];
        //        lst_pro.Add(parent);
        //        lst_id[index] = lst_id[index + 1];
        //        dem++;

        //    }

        //    return lst_pro;
        //}

        //Lấy danh mục sp ngẫu nhiên
        //public List<ParentCategory> getRandomParent()
        //{
        //    var lst_id = db.ParentCategories.Select(x => x.ID).ToArray();
        //    var lst_parent = new List<ParentCategory>();
           
        //    int dem = 0;
        //    var random = new Random();
        //    while (true)
        //    {
        //        if (dem == 3)
        //            break;
        //        long index = random.Next(lst_id.Length - 1);
        //        var parent = new ParentCategory();
        //        var parent_id = lst_id[index];

        //        parent = db.ParentCategories.Find(parent_id);
        //        lst_parent.Add(parent);
        //        lst_id[index] = lst_id[index + 1];
        //        dem++;

        //    }

        //    return lst_parent;
        //}
        ////Chi tiết sản phẩm
        //public ProductDTO getProductDetail(long ID)
        //{
        //    var query = from pro in db.Products
        //                join cet in db.Categories on pro.Category_ID equals cet.ID
        //                select new ProductDTO()
        //                {
        //                    ID = pro.ID,
        //                    Product_Name = pro.Product_Name,
        //                    Product_Code = pro.Product_Code,
        //                    Metatitle = pro.Metatitle.Trim(),
        //                    Object = pro.Object,
        //                    Promotion_Price = pro.Promotion_Price,
        //                    Price = pro.Price,
        //                    Image = pro.Image,
        //                    Desription = pro.Desription,
        //                    Configuration = pro.Configuration,
        //                    Category_Name = cet.Name,
        //                    Category_ID = pro.Category_ID
        //                };
        //    return query.Single(x => x.ID == ID);
        //}


        //Lấy sản phẩm cùng loại
        public IEnumerable<Product> getProductByCategoryID(long? category_id)
        {
            
                return db.Products.Where(x => x.Category_ID == category_id && x.Status == 1).OrderByDescending(x => x.Product_Name).ToList();
            
        }

        //public IEnumerable<Product> getProductByCategory(long? category_id, int page, int pagesize, string order = null)
        //{
        //    if (order != null)
        //    {
        //        if (order == "asc")
        //            return db.Products.Where(x => x.Category_ID == category_id && x.Status == true).OrderBy(x => x.Promotion_Price).ToPagedList(page, pagesize);
        //        else
        //            return db.Products.Where(x => x.Category_ID == category_id && x.Status == true).OrderByDescending(x => x.Promotion_Price).ToPagedList(page, pagesize);
        //    }
        //    else
        //    {
        //        return db.Products.Where(x => x.Category_ID == category_id && x.Status == true).OrderByDescending(x => x.Product_Name).ToPagedList(page, pagesize);
        //    }

        //}

        //thêm review sản phẩm
        public bool addReview(Review entity)
        {
            try
            {
                db.Reviews.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //lấy review sản phẩm
        //public List<ReviewDTO> getReview(long product_id)
        //{
        //    var query = from rev in db.Reviews
        //                join pro in db.Products on rev.Product_ID equals pro.ID
        //                join user in db.Users on rev.User_ID equals user.ID
        //                where rev.Product_ID == product_id
        //                select new ReviewDTO()
        //                {
        //                    ID = rev.ID,
        //                    Content = rev.Content,
        //                    Rating = rev.Rating,
        //                    Fullname = user.Fullname,
        //                    CreatedDate = rev.CreatedDate
        //                };
        //    return query.OrderByDescending(x => x.CreatedDate).ToList();
        //}


        
    }
}