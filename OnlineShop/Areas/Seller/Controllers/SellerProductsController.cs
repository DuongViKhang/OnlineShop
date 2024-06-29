﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using X.PagedList;

namespace OnlineShop.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerProductsController : Controller
    {
        private readonly OnlineShopContext _context;
        private readonly IHostingEnvironment _environment;

        public SellerProductsController(OnlineShopContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Seller/SellerProducts
        public async Task<IActionResult> Index(int? page)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            var onlineShopContext = _context.Products.Where(p => p.SellerId == userId).Include(p => p.Category).OrderBy(n => n.IsActive);
            return View(onlineShopContext.ToPagedList(page ?? 1, 5));
        }

        // GET: Seller/SellerProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Where(p => p.SellerId == userId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id && m.SellerId == userId);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.style = _context.Styles.Where(n => n.ProductId == product.ProductId).ToList();
            return View(product);
        }

        // GET: Seller/SellerProducts/Create
        public IActionResult Create()
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Seller/SellerProducts/Create
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
            if (product.PromotionalPrice >= product.Price)
            {
                ViewBag.mess = "Giá khuyến mãi phải thấp hơn giá gốc";
                return View(product);
            }
            if (ModelState.IsValid)
            {
                if (product.Quantity <= 0)
                {
                    product.Quantity = 1;
                }
                if (product.Sold < 0)
                {
                    product.Sold = 0;
                }
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
                product.SellerId = userId;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Seller/SellerProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Where(p => p.SellerId == userId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id && m.SellerId == userId);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Seller/SellerProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, [Bind("ProductId,SellerId,ProductName,Decription,Price,PromotionalPrice,Quantity,Sold,IsActive,Image,CategoryId,StyleId,Rating,Date,IsDeleted")] Product product)
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

        // GET: Seller/SellerProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Where(p => p.SellerId == userId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id && m.SellerId == userId);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: Seller/SellerProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product.IsActive == 0)
            {
                product.IsActive = 1;
            }
            else if (product.IsActive == 1)
            {
                product.IsActive = 0;
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddStyleAsync(int id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            var product = await _context.Products
                .Where(p => p.SellerId == userId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id && m.SellerId == userId);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.productId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStyle(IFormFile Image, [Bind("StyleId,ProductId, StyleName, Image, IsDeleted")] Style style)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["StyleId"] = new SelectList(_context.Styles, "StyleId", "StyleName");
            foreach (PropertyInfo pi in style.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(style);
                    if (string.IsNullOrEmpty(value))
                    {
                        ViewBag.mess = "Vui lòng điển đẩy đủ thông tin";
                        return View(style);
                    }
                }
            }
            if (_context.Styles.Where(n => n.StyleName == style.StyleName && n.ProductId == style.ProductId).Count() > 0)
            {
                ViewBag.mess = "Loại sản phẩm đã tồn tại";
                return View(style);
            }
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    style.Image = Image.FileName;
                    var uploadDirectory = Path.Combine(_environment.WebRootPath, "upload", "images", "product");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }
                    var path = Path.Combine(uploadDirectory, Image.FileName);
                    using var fileStream = new FileStream(path, FileMode.Create);
                    await Image.CopyToAsync(fileStream);
                }
                _context.Add(style);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "SellerProducts", new { id = style.ProductId });
            }
            return View(style);
        }

        public async Task<IActionResult> EditStyle(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Seller")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            if (id == null)
            {
                return NotFound();
            }

            var style = _context.Styles.FirstOrDefault(m => m.StyleId == id && m.Product.SellerId == userId);
            if (style == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(style);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStyle(int id, IFormFile Image, [Bind("StyleId,ProductId, StyleName, Image, IsDeleted")] Style style)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            foreach (PropertyInfo pi in style.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(style);
                    if (string.IsNullOrEmpty(value))
                    {
                        ViewBag.mess = "Vui lòng điển đẩy đủ thông tin";
                        return View(style);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    style.Image = _context.Styles.AsNoTracking().FirstOrDefault(n => n.StyleId == id).Image;
                    if (Image != null)
                    {
                        style.Image = Image.FileName;
                        var uploadDirectory = Path.Combine(_environment.WebRootPath, "upload", "images", "product");
                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        var path = Path.Combine(uploadDirectory, Image.FileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await Image.CopyToAsync(fileStream);
                    }
                    _context.Update(style);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) 
                { 
                
                        return NotFound();
                }
                return RedirectToAction("Details", "SellerProducts", new { id = style.ProductId });
            }
            return RedirectToAction("Details", "SellerProducts", new { id = style.ProductId });
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var style = await _context.Styles.FindAsync(id);
            if (style.IsDeleted == 0)
            {
                style.IsDeleted = 1;
            }
            else if (style.IsDeleted == 1)
            {
                style.IsDeleted = 0;
            }
            _context.Styles.Update(style);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "SellerProducts", new { id = style.ProductId });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
