using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using OnlineShop.Payment;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using MailKit.Search;
using Microsoft.CodeAnalysis;
using System.Drawing;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly OnlineShopContext _context;
        private readonly IConfiguration _configuration;
        public ProductController(ILogger<ProductController> logger, OnlineShopContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index(int? page, string? categoryName, string? Sort)
        {
            int userId;
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (isNum)
            {
                string roleName = HttpContext.Session.GetString("roleName");
                if (roleName != "Customer")
                {
                    return RedirectToAction("Index", "Home", new { area = roleName });
                }
                ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
                int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
                var query = from s1 in _context.Carts.Where(s1 => s1.UserId == userId)
                            join s2 in _context.CartItems on s1.CartId equals s2.CartId
                            select new OrderCartViewModel
                            {
                                CartItemId = s2.CartItemId,
                                Image = s2.Product.Image,
                                PromotionalPrice = (decimal)s2.Product.PromotionalPrice,
                                ProductName = s2.Product.ProductName,
                                Count = s2.Count,
                                Total = (decimal)s2.Product.PromotionalPrice * s2.Count,
                                StyleName = s2.Style.StyleName
                            };
                List<OrderCartViewModel> lst = query.ToList();
                ViewBag.quantity = lst.Count;
                ViewBag.cartItems = lst;
                ViewBag.totalCartItems = lst.Sum(n => n.Total);
            }
            var productList = _context.Products.Include(p=>p.Category).Where(n => n.IsDeleted == 0 && n.IsActive == 1 && n.Category.IsDeleted==0).AsQueryable();
            if (categoryName != null)
            {
                productList = productList.Where(p => p.Category.CategoryName.Contains(categoryName));

            }
            
            if (Sort != null && Sort.Contains("Mới nhất"))
            {
                productList = productList.OrderByDescending(p => p.Date);
            }
            else if (Sort != null && Sort.Contains("Giá lớn nhất"))
            {
                productList = productList.OrderByDescending(p => p.Price);
            }
            else if (Sort != null && Sort.Contains("Giá thấp nhất"))
            {
                productList = productList.OrderBy(p => p.Price);
            }
            //var categoryList = _context.Categories.ToList();
            //ViewData["Categories"] = categoryList;
            if (TempData["orderSuccess"] != null && (bool)TempData["orderSuccess"])
            {
                ViewBag.orderSuccess = true;
            }
            if (TempData["addToCartSuccess"] != null && (bool)TempData["addToCartSuccess"])
            {
                ViewBag.addToCartSuccess = true;
            }
            var productVM = new ProductViewModel();
            productVM.productList = productList.ToPagedList(page ?? 1, 6);
            ViewBag.Sort = new List<String> { "Mới nhất", "Giá lớn nhất", "Giá thấp nhất" };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Search(int? page, string keyword)
        {
            var results = _context.Products.Where(n => n.IsDeleted == 0 && n.IsActive == 1).Include(p => p.Category)
            .Where(p => p.ProductName.Contains(keyword)).ToPagedList(page ?? 1, 5);
            //var categoryList = _context.Categories.ToList();
            //ViewData["Categories"] = categoryList;
            var productVM = new ProductViewModel();
            productVM.productList = results;
            ViewBag.Sort = new List<String> { "Mới nhất", "Giá lớn nhất", "Giá thấp nhất" };
            return View("Index", productVM);
        }
        public IActionResult Detail(int id, string mess, bool flagComment = false)
        {
            if(mess != null)
            {
                ViewBag.mess = mess;
            }
            int userId;
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.flagComment = flagComment;
            if (isNum)
            {
                string roleName = HttpContext.Session.GetString("roleName");
                if (roleName != "Customer")
                {
                    return RedirectToAction("Index", "Home", new { area = roleName });
                }
                ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
                int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
                var query = from s1 in _context.Carts.Where(s1 => s1.UserId == userId)
                            join s2 in _context.CartItems on s1.CartId equals s2.CartId
                            select new OrderCartViewModel
                            {
                                CartItemId = s2.CartItemId,
                                Image = s2.Product.Image,
                                PromotionalPrice = (decimal)s2.Product.PromotionalPrice,
                                ProductName = s2.Product.ProductName,
                                Count = s2.Count,
                                Total = (decimal)s2.Product.PromotionalPrice * s2.Count,
                                StyleName = s2.Style.StyleName
                            };
                List<OrderCartViewModel> lst = query.ToList();
                ViewBag.quantity = lst.Count;
                ViewBag.cartItems = lst;
                ViewBag.totalCartItems = lst.Sum(n => n.Total);
            }
            var product = _context.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefault(p=>p.ProductId.Equals(id));

            if (product == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }
            List<Style> styles = _context.Styles.Where(n => n.ProductId == id).ToList();
            ViewBag.Styles = styles;
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
		public IActionResult OrderProduct(int styleId, int productId, int quantity)
		{
            if (quantity <= 0)
            {
                string mess = "Số lượng sản phẩm đã chọn phải lớn hơn 0";
                return RedirectToAction("Detail", "Product", new { id = productId, mess = mess });
            }
            if (quantity > _context.Products.FirstOrDefault(n => n.ProductId == productId).Quantity)
            {
                string mess = "Số lượng sản phẩm đã chọn vượt quá số lượng còn lại trong kho";
                return RedirectToAction("Detail", "Product", new { id = productId, mess = mess });
            }
            int userId;
			string roleName = HttpContext.Session.GetString("roleName");
			bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
			if (!isNum)
			{
				return RedirectToAction("SignIn", "Customer", new { area = "Default" });
			}
			if (roleName != "Customer")
			{
				return RedirectToAction("Index", "Home", new { area = "Admin" });
			}
            User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            ViewBag.username = user.UserName;
            int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
            var query = from s1 in _context.Carts.Where(s1 => s1.UserId == userId)
                        join s2 in _context.CartItems on s1.CartId equals s2.CartId
                        select new OrderCartViewModel
                        {
                            CartItemId = s2.CartItemId,
                            Image = s2.Product.Image,
                            PromotionalPrice = (decimal)s2.Product.PromotionalPrice,
                            ProductName = s2.Product.ProductName,
                            ProductId = s2.Product.ProductId,
                            Count = s2.Count,
                            Total = (decimal)s2.Product.PromotionalPrice * s2.Count,
                            StyleName = s2.Style.StyleName
                        };
			
			List<OrderCartViewModel> cartItems = query.ToList();
			
			ViewBag.quantity = cartItems.Count;
            ViewBag.cartItems = cartItems;
            ViewBag.totalCartItems = cartItems.Sum(n => n.Total);
            ViewBag.quantity = _context.CartItems.Where(n => n.CartId == cartId).Count();
            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.ProductName;
            ViewBag.Count = quantity;
            ViewBag.Total = product.PromotionalPrice * quantity;
            ViewBag.Style = _context.Styles.FirstOrDefault(n => n.StyleId == styleId);
            var vouchers = _context.VoucherItems.Include(v => v.Voucher).Where(v => v.UserId == userId && v.IsDeleted != 1).ToList();
            var list_voucher = new List<VoucherItem>();
            foreach (var voucher in vouchers)
            {
                if (voucher.Voucher.IsDeleted == 0 && DateTime.Now <= voucher.Voucher.ExpirationDate)
                {
                    list_voucher.Add(voucher);
                }
            }
            ViewBag.vouchers = list_voucher;
            return View(user);
		}
        [HttpPost]
        public IActionResult ApplyVoucher(int voucherId, int productId, int quantity)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Customer")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            var total = (double?)product.PromotionalPrice * quantity;



            if (voucherId >0)
            {
                var voucherApplied = _context.VoucherItems.Include(v => v.Voucher).FirstOrDefault(v => v.VoucherItemId == voucherId);
                if (voucherApplied.Voucher.DiscountType.Contains("Percent"))
                {
                    var i = voucherApplied.Voucher.Discount / 100;
                    total = total - total * i;
                }
                else
                {
                    total = total - voucherApplied.Voucher.Discount;
                }
            }

            return Json(total);
        }
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OrderProduct(string receiver, string email, string phone, string address, int productId, int count, string paymentOption, int styleId, int voucherSelected)
		{
            int userId = int.Parse(HttpContext.Session.GetString("userId"));
			User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
			Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
			ViewBag.username = user.UserName;
			int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
			var query = from s1 in _context.Carts.Where(s1 => s1.UserId == userId)
						join s2 in _context.CartItems on s1.CartId equals s2.CartId
						select new OrderCartViewModel
						{
							CartItemId = s2.CartItemId,
							Image = s2.Product.Image,
							PromotionalPrice = (decimal)s2.Product.PromotionalPrice,
							ProductName = s2.Product.ProductName,
                            ProductId = s2.Product.ProductId,
							Count = s2.Count,
							Total = (decimal)s2.Product.PromotionalPrice * s2.Count,
							StyleName = s2.Style.StyleName
						};
			List<OrderCartViewModel> cartItems = query.ToList();
			if (receiver == null || email == null | phone == null || address == null)
            {
                ViewBag.mess = "Vui lòng điền đầy đủ thông tin trước khi đặt hàng";           
                ViewBag.quantity = cartItems.Count;
                ViewBag.cartItems = cartItems;
                ViewBag.totalCartItems = cartItems.Sum(n => n.Total);
                ViewBag.quantity = _context.CartItems.Where(n => n.CartId == cartId).Count();
                ViewBag.ProductId = productId;
                ViewBag.ProductName = product.ProductName;
                ViewBag.Count = count;
                ViewBag.Total = product.PromotionalPrice * count;
                ViewBag.Style = _context.Styles.FirstOrDefault(n => n.StyleId == styleId);
                var vouchers = _context.VoucherItems.Include(v => v.Voucher).Where(v => v.UserId == userId).ToList();
                var list_voucher = new List<VoucherItem>();
                foreach (var voucher in vouchers)
                {
                    if (voucher.Voucher.IsDeleted == 0 && DateTime.Now <= voucher.Voucher.ExpirationDate)
                    {
                        list_voucher.Add(voucher);
                    }
                }
                ViewBag.vouchers = list_voucher;
                return View(user);
            }


			if (paymentOption == "4")
            {


				var voucheritem = _context.VoucherItems.FirstOrDefault(v => v.VoucherItemId == voucherSelected);
				if (voucherSelected > 0 && voucheritem != null)
				{
					voucheritem.Quantity -= 1;
					_context.Update(voucheritem);
				}
				Order order = new Order
                {
                    UserId = userId,
                    Receiver = receiver,
                    Email = email,
                    Phone = phone,
                    Address = address,
                    StatusId = 1,
                    IsPay = 0,
                    Date = DateTime.Now,
                    VoucherId = voucheritem!=null?voucheritem.VoucherId:0,
                    IsDeleted = 0,
                    PaymentMethod="Thanh toán khi nhận hàng"
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                int newOrderId = order.OrderId;
                OrderItem orderItem = new OrderItem
                {
                    OrderId = newOrderId,
                    ProductId = productId,
                    Count = count,
                    StyleId = styleId
                };
                Product product1 = _context.Products.FirstOrDefault(n => n.ProductId == productId);
                product1.Quantity -= count;
                product1.Sold += count;
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();
                TempData["orderSuccess"] = true;
                return RedirectToAction("Index", "Product");
            }
            else
            {
                HttpContext.Session.SetString("receiverName", receiver);
                HttpContext.Session.SetString("receiverEmail", email);
                HttpContext.Session.SetString("receiverPhone", phone);
                HttpContext.Session.SetString("orderProduct", productId.ToString());
                HttpContext.Session.SetString("orderCount", count.ToString());
                HttpContext.Session.SetString("orderStyle", styleId.ToString());
                HttpContext.Session.SetString("receiverAddress", address);
                HttpContext.Session.SetString("receiverVoucher", Convert.ToString(voucherSelected));
                var url = URLPayment(int.Parse(paymentOption), voucherSelected, productId, count);
                return Redirect(url);
            }
        }
        public string URLPayment(int TypePaymentVN, int voucherSelected, int productId, int count)
        {
            var urlPayment = "";
            var order = _context.Orders.OrderByDescending(o=>o.OrderId).FirstOrDefault();
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            var totalAmount = (double?)product.PromotionalPrice * count;


            if(voucherSelected > 0) 
            { 
                var voucher = _context.VoucherItems.Include(o => o.Voucher).FirstOrDefault(v=>v.VoucherItemId==voucherSelected);
                if(voucher != null)
                {
                    if (voucher.Voucher.DiscountType == "Percent")
                    {
                        totalAmount = totalAmount - totalAmount * voucher.Voucher.Discount/100;

                    }
                    else if(voucher.Voucher.DiscountType == "Amount")
                    {
                        totalAmount = totalAmount - voucher.Voucher.Discount;
                    }
                }
            }
            //Get Config Info
            string vnp_Returnurl = _configuration["VnpSettings:ProductReturnUrl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VnpSettings:Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VnpSettings:TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _configuration["VnpSettings:HashSecret"]; //Secret Key
                                                                              //Get payment input
                                                                              //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (totalAmount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            var user = _context.Users.FirstOrDefault(u => u.UserId == order.UserId);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng của user :" + user.UserName);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", Convert.ToString(order.OrderId + 1)); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }

        public async Task<IActionResult> VnPayReturn()
        {

            int userId;
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
            var lstNew = _context.CartItems.Where(n => n.Cart.UserId == userId && n.IsDeleted == 0).ToList();
            var query = from s1 in _context.Carts.Where(s1 => s1.UserId == userId)
                        join s2 in _context.CartItems on s1.CartId equals s2.CartId
                        select new OrderCartViewModel
                        {
                            CartItemId = s2.CartItemId,
                            Image = s2.Product.Image,
                            PromotionalPrice = (decimal)s2.Product.PromotionalPrice,
                            ProductName = s2.Product.ProductName,
                            Count = s2.Count,
                            Total = (decimal)s2.Product.PromotionalPrice * s2.Count,
                            StyleName = s2.Style.StyleName
                        };
            List<OrderCartViewModel> lst = query.ToList();
            ViewBag.quantity = lst.Count;
            ViewBag.cartItems = lst;
            ViewBag.totalCartItems = lst.Sum(n => n.Total);

            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = _configuration["VnpSettings:HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (var queryParam in vnpayData)
                {
                    // Lấy tất cả dữ liệu query string
                    string key = queryParam.Key;
                    string value = queryParam.Value;

                    // Chỉ xử lý các tham số có tên bắt đầu bằng "vnp_"
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value);
                    }
                }
                string orderId = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = Request.Query["vnp_SecureHash"];
                string TerminalID = Request.Query["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                string bankCode = Request.Query["vnp_BankCode"];

                string receiver = HttpContext.Session.GetString("receiverName");

                string email = HttpContext.Session.GetString("receiverEmail");


                string phone = HttpContext.Session.GetString("receiverPhone");


                string address = HttpContext.Session.GetString("receiverAddress");

                int voucherId = Convert.ToInt32(HttpContext.Session.GetString("receiverVoucher"));

                int productId = Convert.ToInt32(HttpContext.Session.GetString("orderProduct"));
                int count = Convert.ToInt32(HttpContext.Session.GetString("orderCount"));
                int styleId = Convert.ToInt32(HttpContext.Session.GetString("orderStyle"));

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    //Thanh toan thanh cong
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var order = _context.Orders.FirstOrDefault(x => x.OrderId == int.Parse(orderId));
                        if (order == null)
                        {                            
							var voucheritem = _context.VoucherItems.FirstOrDefault(v => v.VoucherItemId == voucherId);
							if(voucherId > 0 && voucheritem!=null)
                            {
								voucheritem.Quantity -= 1;
								_context.Update(voucheritem);
							}
							Order orderNew = new Order
                            {
                                UserId = userId,
                                Receiver = receiver,
                                Email = email,
                                Phone = phone,
                                Address = address,
                                StatusId = 1,
                                IsPay = 1,
                                VoucherId = voucheritem != null ? voucheritem.VoucherId : 0,
                                Date = DateTime.Now,
                                IsDeleted = 0,
                                PaymentMethod="Thanh toán online"
                            };
                            _context.Orders.Add(orderNew);
                            await _context.SaveChangesAsync();
                            int newOrderId = orderNew.OrderId;

                                OrderItem orderItem = new OrderItem
                                {
                                    OrderId = newOrderId,
                                    ProductId = productId,
                                    Count = count,
                                    StyleId = styleId,
                                };
                                _context.OrderItems.Add(orderItem);
                                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                                product.Quantity -=count;
                                product.Sold += count;
                                _context.Update(product); 
                                await _context.SaveChangesAsync();

                        }
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Đã hủy thanh toán nên hủy đơn hàng";
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                        return View();
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán: " + vnp_Amount.ToString("#,##0") + "VNĐ";
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getAllCommnentByProductId(int id)
        {
            var comments = await _context.Comments.Where(c=>c.ProductId == id && c.IsDeleted==0).OrderByDescending(c=>c.Date).ToListAsync();
            if(comments.Any())
            {
                // Chuyển đổi danh sách List<Comment> thành chuỗi JSON
                var jsonComments = JsonConvert.SerializeObject(comments);
                return Ok(comments);
            }
            else
            {
                return BadRequest("không có comments");
            }
        }
        [HttpPost]
		public IActionResult CreateComment(string content, int rating, int productId, int userId)
		{
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                var commnent = new Comment();
                commnent.UserId = userId;
                commnent.ProductId = productId;
                commnent.Content = content;
                commnent.Rate = rating;
                commnent.Date = DateTime.Now;
                commnent.IsDeleted = 0;
                if(product.Rating > 0)
                {
                    product.Rating = (product.Rating + rating) / 2;
                }
                _context.Products.Update(product);
                _context.Comments.Add(commnent);
                _context.SaveChanges();
                return RedirectToAction("Detail", "Product", new { id = productId, mess = "" });
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
		}
	}
}
