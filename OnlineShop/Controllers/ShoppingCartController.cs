﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using OnlineShop.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using OnlineShop.Payment;
using Org.BouncyCastle.Asn1.X9;

namespace OnlineShop.Controllers
{
	public class ShoppingCartController : Controller
	{
		private readonly OnlineShopContext _context;
		private readonly ILogger<ShoppingCartController> _logger;
		private readonly IConfiguration _configuration;

		public ShoppingCartController(OnlineShopContext context, ILogger<ShoppingCartController> logger, IConfiguration configuration)
		{
			_context = context;
			_logger = logger;
			_configuration = configuration;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddToCart(int productId, int count, int styleId)
		{
            if (count <= 0)
            {
                string mess = "Số lượng sản phẩm đã chọn phải lớn hơn 0";
                return RedirectToAction("Detail", "Product", new { id = productId, mess = mess });
            }
            if (count > _context.Products.FirstOrDefault(n => n.ProductId == productId).Quantity)
            {
                string mess = "Số lượng sản phẩm đã chọn vượt quá số lượng còn lại trong kho";
                return RedirectToAction("Detail", "Product", new { id = productId, mess = mess });
            }
            int userId;
			bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
			if (!isNum)
			{
				return RedirectToAction("SignIn", "Customer", new { area = "Default" });
			}
			ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
			try
			{
				CartItem cartItem = _context.CartItems.FirstOrDefault(n => n.ProductId == productId && n.IsDeleted == 0 && n.Cart.UserId == userId && n.StyleId == styleId);

				if (cartItem != null)
				{
					cartItem.Count += count;
					_context.Update(cartItem);
				}
				else
				{
					int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
					cartItem = new CartItem
					{
						CartId = cartId,
						ProductId = productId,
						Count = count,
						Date = DateTime.Now,
						StyleId = styleId,
						IsDeleted = 0
					};
					_context.CartItems.Add(cartItem);
				}
				_context.SaveChangesAsync();
                TempData["addToCartSuccess"] = true;
                return RedirectToAction("Index", "Product");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding product to cart.");
				TempData["ErrorMessage"] = "Error adding the product to the cart.";
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		public IActionResult UpdateCart(int cartItemId, int count)
		{
			int userId;
			bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
			if (!isNum)
			{
				return BadRequest();
			}
			CartItem cartItem = _context.CartItems.FirstOrDefault(n => n.CartItemId == cartItemId && n.IsDeleted == 0 && n.Cart.UserId == userId);
			if (count < 1)
			{
				_context.CartItems.Remove(cartItem);
				_context.SaveChanges();
				return Ok();
			}
			Product product = _context.Products.FirstOrDefault(n => n.ProductId == cartItem.ProductId);
			if (count > product.Quantity)
			{
				return BadRequest();
			}
			ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
			try
			{
				if (cartItem != null)
				{
					cartItem.Count = count;
					_context.Update(cartItem);
				}

				_context.SaveChanges();
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding product to cart.");
				TempData["ErrorMessage"] = "Error adding the product to the cart.";
				return BadRequest();
			}
		}

		public IActionResult Index()
		{
			int userId;
			string roleName = HttpContext.Session.GetString("roleName");
			bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
			if (!isNum)
			{
				return RedirectToAction("SignIn", "Customer", new { area = "Default" });
			}
			if(roleName != "Customer")
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
							StyleName = s2.Style.StyleName,
							ProductId = s2.Product.ProductId,
							Count = s2.Count,
							Total = (decimal)s2.Product.PromotionalPrice * s2.Count
						};
			List<OrderCartViewModel> cartItems = query.ToList();
			ViewBag.quantity = cartItems.Count;
			ViewBag.cartItems = cartItems;
			ViewBag.totalCartItems = cartItems.Sum(n => n.Total);
            if (TempData["quantityError"] != null)
            {
                ViewBag.quantityError = _context.Products.FirstOrDefault(p=>p.ProductId==(int)TempData["quantityError"]);
            }
            return View();
		}

		[HttpPost, ActionName("Remove")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Remove(int id)
		{
			CartItem cartItems = _context.CartItems.FirstOrDefault(n => n.CartItemId == id);
			Product product = _context.Products.FirstOrDefault(n => n.ProductId == cartItems.ProductId);
			product.Quantity += cartItems.Count;
			_context.Products.Update(product);
			_context.CartItems.Remove(cartItems);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult OrderCart(int? voucherId)
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
            foreach (var item in query.ToList())
            {
                var pro = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (pro.Quantity < item.Count)
                {
                    TempData["quantityError"] = pro.ProductId;
                    return RedirectToAction("Index");
                }
            }

            List<OrderCartViewModel> cartItems = query.ToList();
			User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
			var orderVM = new OrderVIewModel();
			ViewBag.username = user.UserName;
			int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
			ViewBag.quantity = _context.CartItems.Where(n => n.CartId == cartId).Count();
			ViewBag.cartItems = cartItems;
			var total = (double?)cartItems.Sum(n => n.Total);
            

            if (voucherId != null)
			{
				var voucherApplied = _context.VoucherItems.Include(v => v.Voucher).FirstOrDefault(v => v.VoucherItemId == voucherId);
				if (voucherApplied.Voucher.DiscountType.Contains("Percent"))
				{
                    var i = voucherApplied.Voucher.Discount/100;
					total = total - total * i;
				}
				else
				{
					total = total - voucherApplied.Voucher.Discount;
				}
               
            }
			ViewBag.totalCartItems = total;

            var vouchers = _context.VoucherItems.Include(v=>v.Voucher).Where(v=>v.UserId==userId && v.IsDeleted!=1).ToList();
            var list_voucher = new List<VoucherItem>();
            foreach(var voucher in vouchers)
            {               
                if (voucher.Voucher.IsDeleted == 0 && DateTime.Now <= voucher.Voucher.ExpirationDate && voucher.Quantity>0)
                {
                    list_voucher.Add(voucher);
                }
            }
			orderVM.vouchers = vouchers;			
            ViewBag.vouchers = list_voucher;
            orderVM.User = user;
            return View(user);
		}

        [HttpPost]
        public IActionResult ApplyVoucher(int voucherId)
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
            List<OrderCartViewModel> cartItems = query.ToList();
            User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
            var orderVM = new OrderVIewModel();
            ViewBag.username = user.UserName;
            int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
            ViewBag.quantity = _context.CartItems.Where(n => n.CartId == cartId).Count();
            ViewBag.cartItems = cartItems;
            var total = (double?)cartItems.Sum(n => n.Total);


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
		public async Task<IActionResult> OrderCart(string receiver, string email, string phone, string address, string paymentOption, int voucherSelected)
        {
			int userId = int.Parse(HttpContext.Session.GetString("userId"));
            if (receiver == null || email == null | phone == null || address == null)
			{
				ViewBag.mess = "Vui lòng điền đầy đủ thông tin trước khi đặt hàng";
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
				List<OrderCartViewModel> cartItems = query.ToList();
				User user = _context.Users.Where(n => n.UserId == userId).FirstOrDefault();
				ViewBag.username = user.UserName;
				int cartId = _context.Carts.FirstOrDefault(n => n.UserId == userId).CartId;
				ViewBag.quantity = _context.CartItems.Where(n => n.CartId == cartId).Count();
				ViewBag.cartItems = cartItems;
				ViewBag.totalCartItems = cartItems.Sum(n => n.Total);
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
                    VoucherId= voucheritem != null ? voucheritem.VoucherId : 0,
                    IsDeleted = 0,
                    PaymentMethod="Thanh toán khi nhận hàng"
                };
                
				_context.Orders.Add(order);
                await _context.SaveChangesAsync();
                int newOrderId = order.OrderId;
                var lst = _context.CartItems.Where(n => n.Cart.UserId == userId && n.IsDeleted == 0).ToList();
                foreach (CartItem item in lst)
                {   
                    OrderItem orderItem = new OrderItem
                    {
                        OrderId = newOrderId,
                        ProductId = item.ProductId,
                        Count = item.Count,
						StyleId = item.StyleId
					};
                    _context.OrderItems.Add(orderItem);
                    _context.CartItems.Remove(item);
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    product.Quantity -= item.Count;
                    product.Sold += item.Count;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Product");
            }
			else
			{
                HttpContext.Session.SetString("receiverName", receiver);
                HttpContext.Session.SetString("receiverEmail", email);
                HttpContext.Session.SetString("receiverPhone", phone);
                HttpContext.Session.SetString("receiverAddress", address);
                HttpContext.Session.SetString("receiverVoucher", Convert.ToString(voucherSelected));
                var url = URLPayment(int.Parse(paymentOption), voucherSelected);
                return Redirect(url);
			}
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
							if (voucherId > 0 && voucheritem != null)
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
                                PaymentMethod= "Thanh toán online"
                            };
                            _context.Orders.Add(orderNew);
                            await _context.SaveChangesAsync();
                            int newOrderId = orderNew.OrderId;
                            
                            foreach (CartItem item in lstNew)
                            {
                                OrderItem orderItem = new OrderItem
                                {
                                    OrderId = newOrderId,
                                    ProductId = item.ProductId,
                                    Count = item.Count,
                                };
                                _context.OrderItems.Add(orderItem);
                                _context.CartItems.Remove(item);
                                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                                product.Quantity -= item.Count;
                                product.Sold += item.Count;
                                _context.Products.Update(product);
                                await _context.SaveChangesAsync();
                            }
                        }                      
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
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

        public string URLPayment(int TypePaymentVN, int voucherSelected)
		{
            var urlPayment = "";
            int userId;
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
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

            var totalAmount = (double?)Convert.ToDouble(lst.Sum(n => n.Total));
            if (voucherSelected > 0)
            {
                var voucher = _context.VoucherItems.Include(o => o.Voucher).FirstOrDefault(v => v.VoucherItemId == voucherSelected);
                if (voucher != null)
                {
                    if (voucher.Voucher.DiscountType == "Percent")
                    {
                        totalAmount = totalAmount - totalAmount * voucher.Voucher.Discount/100;

                    }
                    else if (voucher.Voucher.DiscountType == "Amount")
                    {
                        totalAmount = totalAmount - voucher.Voucher.Discount;
                    }
                }
            }
            //Get Config Info
            string vnp_Returnurl = _configuration["VnpSettings:ReturnUrl"]; //URL nhan ket qua tra ve 
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
			var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng của user :" + user.UserName);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
			var orderLastId = _context.Orders.OrderByDescending(x=>x.OrderId).FirstOrDefault().OrderId;
            vnpay.AddRequestData("vnp_TxnRef", (orderLastId+1).ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                                                                            //Add Params of 2.1.0 Version
                                                                            //Billing
            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
			//log.InfoFormat("VNPAY URL: {0}", paymentUrl);
			return urlPayment;
        }
    }
}
