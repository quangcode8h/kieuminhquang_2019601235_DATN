using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Models.DTO
{
    public class ReviewDTO
    {
        public long ID { get; set; }
        public string Content { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> User_ID { get; set; }
        public Nullable<long> Product_ID { get; set; }
        public Nullable<bool> Status { get; set; }

        public string Fullname { get; set; }
        public Nullable<int> Count_Rate { get; set; }
        public Nullable<double> percent { get; set; }
    }
}