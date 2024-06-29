using OnlineShop.Models;
using System.Collections.Generic;

namespace OnlineShop.ViewModels
{
    public class OrderVIewModel
    {
        public User User { get; set; }
        public int VoucherId { get; set; }
        public List<VoucherItem> vouchers { get; set; }
    }
}
