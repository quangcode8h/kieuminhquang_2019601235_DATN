//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Watch.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order_Detail
    {
        public long ID { get; set; }
        public Nullable<long> Product_ID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<long> Order_ID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> DiscountRate { get; set; }
        public Nullable<int> Amount { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
