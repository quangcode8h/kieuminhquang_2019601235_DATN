using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Models.DTO
{
    public class WarrantyDTO
    {
        public long ID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Email { get; set; }
        public Nullable<long> Product_ID { get; set; }
        public string Date_Start { get; set; }
        public string Date_End { get; set; }
        public string Description { get; set; }
        public Nullable<long> Admin_ID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}