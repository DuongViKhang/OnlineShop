using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class VoucherItem
    {
        public int VoucherItemId { get; set; }
        public int VoucherId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public int IsDeleted { get; set; }
        public virtual User User { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
