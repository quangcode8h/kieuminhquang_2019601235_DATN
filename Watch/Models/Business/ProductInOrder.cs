using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Models.Business
{
    public class ProductInOrder
    {
        public long id { get; set; }
        public long OrderID { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int amount { get; set; }
    }
}