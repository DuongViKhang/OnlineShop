using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Voucher
    {
        public int VoucherId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên voucher!")]
        public string VoucherName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền mô tả!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại voucher!")]
        public string DiscountType { get; set; }
        [Required(ErrorMessage = "Vui lòng điền mục giảm giá!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mục giảm giá phải lớn hơn 0!")]
        public double? Discount { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày phát hành!")]
        public DateTime? ReleaseDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn!")]
        public DateTime? ExpirationDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trạng thái!")]
        public int IsDeleted { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
