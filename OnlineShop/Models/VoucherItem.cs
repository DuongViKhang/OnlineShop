using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace OnlineShop.Models
{
    public partial class VoucherItem
    {
        public int VoucherItemId { get; set; }
        public int VoucherId { get; set; }
        public int UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền số lượng phát hành!")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phát hành phải lớn hơn 0!")]
        public int? Quantity { get; set; }
        public int IsDeleted { get; set; }
        public virtual User User { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
