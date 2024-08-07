﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly OnlineShopContext _context;
        private readonly IHostingEnvironment _environment;

        public AdminProductsController(OnlineShopContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(int? page, string category, string keyword)
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
            ViewBag.categories = _context.Categories.Select(n => n.CategoryName);
            if(keyword != null)
            {
                ViewBag.category = "Tất cả sản phẩm";
                var onlineShopContext = _context.Products.Where(n => n.ProductName.Contains(keyword)).OrderByDescending(n => n.IsDeleted).Include(p => p.Category);
                return View(onlineShopContext.ToPagedList(page ?? 1, 5));
            }
            if (category == null || category == "All") 
            { 
                ViewBag.category = "Tất cả sản phẩm";
                var onlineShopContext = _context.Products.OrderByDescending(n => n.IsDeleted).Include(p => p.Category);
                return View(onlineShopContext.ToPagedList(page ?? 1, 5));
            }
            else if(category != null)
            {
                ViewBag.category = category;
                var onlineShopContext = _context.Products.Where(n => n.Category.CategoryName == category).OrderByDescending(n => n.IsDeleted).Include(p => p.Category);
                return View(onlineShopContext.ToPagedList(page ?? 1, 5));
            }
            return View();
        }

        // GET: Admin/AdminProducts/Details/5
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

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Image, [Bind("ProductId,ProductName,Decription,Price,PromotionalPrice,Quantity,Sold,IsActive,Image,CategoryId,StyleId,Rating,Date,IsDeleted")] Product product)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["StyleId"] = new SelectList(_context.Styles, "StyleId", "StyleName");
            foreach (PropertyInfo pi in product.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(product);
                    if (string.IsNullOrEmpty(value))
                    {
                        ViewBag.mess = "Vui lòng điển đẩy đủ thông tin";
                        return View(product);
                    }
                }
            }
            if (_context.Products.Where(n => n.ProductName == product.ProductName).Count() > 0)
            {
                ViewBag.mess = "Sản phẩm đã tồn tại";
                return View(product);
            }
            if (product.PromotionalPrice <= 0 || product.Price <= 0)
            {
                ViewBag.mess = "Giá khuyến mãi và giá gốc phải lớn hơn 0";
                return View(product);
            }
            if(product.PromotionalPrice >= product.Price)
            {
                ViewBag.mess = "Giá khuyến mãi phải thấp hơn giá gốc";
                return View(product);
            }
            if (product.Quantity <= 0)
            {
                ViewBag.mess = "Số lượng đã bán không được bé hơn 0";
                return View(product);
            }
            if (product.Sold < 0)
            {
                ViewBag.mess = "Số lượng đã bán không được bé hơn 0";
                return View(product);
            }
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    product.Image = Image.FileName;
                    var uploadDirectory = Path.Combine(_environment.WebRootPath, "upload", "images", "product");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }
                    var path = Path.Combine(uploadDirectory, Image.FileName);
                    using var fileStream = new FileStream(path, FileMode.Create);
                    await Image.CopyToAsync(fileStream);
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
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

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, [Bind("ProductId, SellerId, ProductName, Decription, Price, PromotionalPrice, Quantity, Sold, IsActive, Image, CategoryId, StyleId, Rating, Date, IsDeleted")] Product product)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            foreach (PropertyInfo pi in product.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(product);
                    if (string.IsNullOrEmpty(value))
                    {
                        ViewBag.mess = "Vui lòng điển đẩy đủ thông tin";
                        return View();
                    }
                }
            }
            if (product.PromotionalPrice <= 0 || product.Price <= 0)
            {
                ViewBag.mess = "Giá khuyến mãi và giá gốc phải lớn hơn 0";
                return View(product);
            }
            if (product.PromotionalPrice >= product.Price)
            {
                ViewBag.mess = "Giá khuyến mãi phải thấp hơn giá gốc";
                return View(product);
            }
            if (product.Quantity <= 0)
            {
                ViewBag.mess = "Số lượng đã bán không được bé hơn 0";
                return View(product);
            }
            if (product.Sold < 0)
            {
                ViewBag.mess = "Số lượng đã bán không được bé hơn 0";
                return View(product);
            }
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Image = _context.Products.AsNoTracking().FirstOrDefault(n => n.ProductId == id).Image;
                    if (Image != null)
                    {
                        product.Image = Image.FileName;
                        var uploadDirectory = Path.Combine(_environment.WebRootPath, "upload", "images", "product");
                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        var path = Path.Combine(uploadDirectory, Image.FileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await Image.CopyToAsync(fileStream);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id, int status)
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

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.status = status;
            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product.IsDeleted == 0)
            {
                product.IsDeleted = 1;
            }
            else if (product.IsDeleted == 1 || product.IsDeleted == 2)
            {
                product.IsDeleted = 0;
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
