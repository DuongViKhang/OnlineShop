using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Voucher
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public double? Discount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int IsDeleted { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
