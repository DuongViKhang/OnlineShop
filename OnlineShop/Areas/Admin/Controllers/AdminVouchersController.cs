using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using X.PagedList;
using System.Reflection;
using System.Globalization;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminVouchersController : Controller
    {
        private readonly OnlineShopContext _context;

        public AdminVouchersController(OnlineShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminVouchers
        public async Task<IActionResult> Index(int? page, string discountType, string keyword)
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
            if (keyword != null)
            {
                ViewBag.discountType = "Tất cả voucher";
                return View(_context.Vouchers.Where(n => n.VoucherId != 0 && n.VoucherName.Contains(keyword)).ToPagedList(page ?? 1, 5));
            }
            if (discountType == null || discountType == "All")
            {
                ViewBag.discountType = "Tất cả voucher";
                return View(_context.Vouchers.Where(n => n.VoucherId != 0).ToPagedList(page ?? 1, 5));
            }
            else if (discountType != null)
            {
                ViewBag.discountType = discountType;
                return View(_context.Vouchers.Where(n => n.VoucherId != 0 && n.DiscountType == discountType).ToPagedList(page ?? 1, 5));
            }
            return View();
        }

        // GET: Admin/AdminVouchers/Details/5
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

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.VoucherId == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // GET: Admin/AdminVouchers/Create
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
            return View();
        }

        // POST: Admin/AdminVouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoucherId,VoucherName,Description,DiscountType,Discount,ReleaseDate,ExpirationDate,IsDeleted")] Voucher voucher)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (ModelState.IsValid && voucher.ExpirationDate > voucher.ReleaseDate)
            {
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (voucher.ExpirationDate < voucher.ReleaseDate)
            {
                ViewBag.checkDate = "Ngày hết hạn phải sau ngày phát hành!";
            }
            double checkValidNumber;
            var isValidNumber = double.TryParse(Convert.ToString(voucher.Discount, CultureInfo.InvariantCulture), out checkValidNumber);
            if (!isValidNumber)
            {
                ViewBag.checkValidNumber = "Mục giảm giá không hợp lệ!";
            }
            ViewBag.mess = "Thêm voucher thất bại!";
            return View(voucher);
        }

        // GET: Admin/AdminVouchers/Edit/5
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

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }

        // POST: Admin/AdminVouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoucherId,VoucherName,Description,DiscountType,Discount,ReleaseDate,ExpirationDate,IsDeleted")] Voucher voucher)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (ModelState.IsValid && voucher.ExpirationDate > voucher.ReleaseDate)
            {
                _context.Update(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (voucher.ExpirationDate < voucher.ReleaseDate)
            {
                ViewBag.checkDate = "Ngày hết hạn phải sau ngày phát hành!";
            }
            ViewBag.mess = "Chỉnh sửa voucher thất bại!";
            return View(voucher);
        }
        // GET: Admin/AdminVouchers/Release
        public IActionResult Release(int id)
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
            ViewBag.Voucher = _context.Vouchers.Find(id);
            ViewBag.Customers = _context.Users.Where(n => n.Role.RoleName == "Customer").ToList();
            return View();
        }
        // POST: Admin/AdminVouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(string option, [Bind("VoucherItemId,VoucherId,UserId,Quantity,IsDeleted")] VoucherItem voucherItem)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (ModelState.IsValid)
            {
                if (option != "All")
                {
                    var check = _context.VoucherItems.Where(n => n.UserId == Int32.Parse(option) && n.VoucherId == voucherItem.VoucherId).FirstOrDefault();
                    if (check != null)
                    {
                        if(check.Quantity > 0)
                        {
                            check.Quantity += voucherItem.Quantity;
                            _context.Update(check);
                        }
                    }
                    else
                    {
                        voucherItem.UserId = Int32.Parse(option);
                        _context.Add(voucherItem);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else if (option == "All")
                {
                    var customers = _context.Users.Where(n => n.Role.RoleName == "Customer").ToList();
                    foreach(var customer in customers)
                    {
                        var newVoucherItem = new VoucherItem
                        {
                            VoucherId = voucherItem.VoucherId,
                            UserId = customer.UserId,
                            Quantity = voucherItem.Quantity,
                            IsDeleted = voucherItem.IsDeleted
                        };
                        var check = _context.VoucherItems.Where(n => n.UserId == newVoucherItem.UserId 
                                                                && n.VoucherId == newVoucherItem.VoucherId).FirstOrDefault();
                        if (check != null)
                        {
                            if (check.Quantity > 0)
                            {
                                check.Quantity += newVoucherItem.Quantity;
                                _context.Update(check);
                            }
                        }
                        else
                        {
                            _context.Add(newVoucherItem);
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.mess = "Phát hành voucher thất bại!";
            ViewBag.Voucher = _context.Vouchers.Find(voucherItem.VoucherId); 
            ViewBag.Customers = _context.Users.Where(n => n.Role.RoleName == "Customer").ToList();
            return View(voucherItem);
        }
    }
}
