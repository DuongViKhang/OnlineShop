﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OnlineShop.Models;
using X.PagedList;
using OnlineShop.ViewModels;
using System.Globalization;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly OnlineShopContext _context;

        public AdminOrdersController(OnlineShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminOrders
        public async Task<IActionResult> Index(int? page, string statusOrders, int? paymentStatus, string keyword)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            var onlineShopContext = _context.Orders
                                    .Include(o => o.Status)
                                    .Include(o => o.User)
                                    .AsQueryable();
            if (statusOrders != null)
            {
                ViewBag.statusOrder = statusOrders;
                onlineShopContext = onlineShopContext.Where(n => n.Status.StatusName == statusOrders);
            }
            if (paymentStatus != null)
            {
                if (paymentStatus == 1)
                {
                    ViewBag.paymentStatus = "Đã thanh toán";
                }
                else if (paymentStatus == 0)
                {
                    ViewBag.paymentStatus = "Chưa thanh toán";
                }
                onlineShopContext = onlineShopContext.Where(n => n.IsPay == paymentStatus);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.keyword = keyword;
                var cultureInfo = new CultureInfo("de-DE");
                DateTime dt;
                bool isDateParsed = DateTime.TryParse(keyword, cultureInfo, DateTimeStyles.None, out dt);
                int num;
                bool isNumber = int.TryParse(keyword, out num);
                if (isDateParsed)
                {
                    onlineShopContext = onlineShopContext.Where(n => n.Date.HasValue &&
                                                                     n.Date.Value.Year == dt.Year &&
                                                                     n.Date.Value.Month == dt.Month &&
                                                                     n.Date.Value.Day == dt.Day);
                }
                else
                {
                    onlineShopContext = onlineShopContext.Where(n => n.OrderId.ToString().Contains(keyword) ||
                                                                    n.User.Email.Contains(keyword));
                }
            }

            onlineShopContext = onlineShopContext.OrderByDescending(o => o.StatusId == 1)
                                                 .ThenByDescending(o => o.Date);
            return View(onlineShopContext.ToPagedList(page ?? 1, 5));
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var query = from s1 in _context.OrderItems.Where(s1 => s1.OrderId == id)
                        select new OrderCartViewModel
                        {
                            ProductName = s1.Product.ProductName,
                            Count = s1.Count,
                            Total = (decimal)s1.Product.PromotionalPrice * s1.Count
                        };
            List<OrderCartViewModel> lst = query.ToList();
            var total = (double?)lst.Sum(n => n.Total);
            if (order.VoucherId > 0)
            {
                if (order.Voucher.DiscountType.Contains("Percent"))
                {
                    var i = order.Voucher.Discount / 100;
                    total = total - total * i;
                }
                else
                {
                    total = total - order.Voucher.Discount;
                }
            }
            ViewBag.total = total;
            ViewBag.lst = lst;
            return View(order);
        }

        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            ViewData["StatusId"] = new SelectList(_context.StatusOrders, "StatusId", "StatusName");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,Receiver,Address,Phone,StatusId,IsPay,Email,Date,IsDeleted")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusId"] = new SelectList(_context.StatusOrders, "StatusId", "StatusName", order.StatusId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Address", order.UserId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var query = from s1 in _context.OrderItems.Where(s1 => s1.OrderId == id)
                        select new OrderCartViewModel
                        {
                            ProductName = s1.Product.ProductName,
                            Count = s1.Count,
                            Total = (decimal)s1.Product.PromotionalPrice * s1.Count
                        };
            List<OrderCartViewModel> lst = query.ToList();
            var total = (double?)lst.Sum(n => n.Total);
            if (order.VoucherId > 0)
            {
                if (order.Voucher.DiscountType.Contains("Percent"))
                {
                    var i = order.Voucher.Discount / 100;
                    total = total - total * i;
                }
                else
                {
                    total = total - order.Voucher.Discount;
                }
            }
            ViewBag.total = total;
            ViewBag.lst = lst;
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.StatusId = 7;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var query = from s1 in _context.OrderItems.Where(s1 => s1.OrderId == id)
                        select new OrderCartViewModel
                        {
                            ProductName = s1.Product.ProductName,
                            Count = s1.Count,
                            Total = (decimal)s1.Product.PromotionalPrice * s1.Count
                        };
            List<OrderCartViewModel> lst = query.ToList();
            var total = (double?)lst.Sum(n => n.Total);
            if (order.VoucherId > 0)
            {
                if (order.Voucher.DiscountType.Contains("Percent"))
                {
                    var i = order.Voucher.Discount / 100;
                    total = total - total * i;
                }
                else
                {
                    total = total - order.Voucher.Discount;
                }
            }
            ViewBag.total = total;
            ViewBag.lst = lst;
            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.StatusId = 4;
            _context.Update(order);
            await _context.SaveChangesAsync();
            List<OrderItem> orderItems = _context.OrderItems.Where(n => n.OrderId == id).ToList();
            foreach(OrderItem orderItem in orderItems)
            {
                Product product = _context.Products.FirstOrDefault(n => n.ProductId == orderItem.ProductId);
                product.Quantity += orderItem.Count;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var query = from s1 in _context.OrderItems.Where(s1 => s1.OrderId == id)
                        select new OrderCartViewModel
                        {
                            ProductName = s1.Product.ProductName,
                            Count = s1.Count,
                            Total = (decimal)s1.Product.PromotionalPrice * s1.Count
                        };
            List<OrderCartViewModel> lst = query.ToList();
            var total = (double?)lst.Sum(n => n.Total);
            if (order.VoucherId > 0)
            {
                if (order.Voucher.DiscountType.Contains("Percent"))
                {
                    var i = order.Voucher.Discount / 100;
                    total = total - total * i;
                }
                else
                {
                    total = total - order.Voucher.Discount;
                }
            }
            ViewBag.total = total;
            ViewBag.lst = lst;
            return View(order);
        }

        [HttpPost, ActionName("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.StatusId = 3;
            order.IsPay = 1;
            _context.Update(order);
            await _context.SaveChangesAsync();

            var orderItems = _context.OrderItems.Where(o => o.OrderId == id).ToList();

            foreach (var item in orderItems)
            {
                Product product = _context.Products.SingleOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Sold += item.Count;
                    _context.Update(product);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
