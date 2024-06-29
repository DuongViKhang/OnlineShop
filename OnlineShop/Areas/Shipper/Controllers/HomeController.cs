using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.AspNetCore.Hosting;

namespace OnlineShop.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    public class HomeController : Controller
    {
        private readonly OnlineShopContext _context;
        private readonly IHostingEnvironment _environment;
        public HomeController(OnlineShopContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index(int? page, string keyword=null)
        {
            int userId;
            IOrderedQueryable<Order> orderList;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Shipper")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            var user = _context.Users.FirstOrDefault(n => n.UserId == userId);
           
            if (keyword != null)
            {
                orderList = _context.Orders.Include(o => o.Status).Include(o => o.User).Where(o => o.StatusId == 7 && (o.Address.ToLower().Contains(keyword.ToLower())|| o.Phone.ToLower().Contains(keyword.ToLower()))).OrderByDescending(o => o.Date);
            }
            else
            {
                orderList = _context.Orders.Include(o => o.Status).Include(o => o.User).Where(o => o.StatusId == 7).OrderByDescending(o => o.Date);
            }
            foreach (var order in orderList)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem.Product.SellerId == user.SellerId)
                    {
                        orderList.Where(o => o.OrderId == orderItem.OrderId);
                    }
                }

            }
            return View(orderList.ToPagedList(page ?? 1, 5));
        }
        public async Task<IActionResult> ReceiveOrderList(int? page, string keyword = null)
        {
            int userId;
            IOrderedQueryable<Order> orderList;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Shipper")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
           
            if (keyword != null)
            {
                orderList = _context.Orders.Include(o => o.Status).Include(o => o.User).Where(o => (o.StatusId == 2 || o.StatusId == 6) && o.ShipperId == userId && (o.Address.ToLower().Contains(keyword.ToLower()) || o.Phone.ToLower().Contains(keyword.ToLower()))).OrderByDescending(o => o.Date);
            }
            else
            {
                orderList = _context.Orders.Include(o => o.Status).Include(o => o.User).Where(o => (o.StatusId == 2 || o.StatusId == 6) && o.ShipperId == userId).OrderByDescending(o => o.Date);
            }
            return View(orderList.ToPagedList(page ?? 1, 5));
        }
        public async Task<IActionResult> Details(int? id)
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Shipper")
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
            ViewBag.total = lst.Sum(n => n.Total);
            ViewBag.lst = lst;
            return View(order);
        }

        public async Task<IActionResult> ReceiveDelivery(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.StatusId = 2;

            order.ShipperId = int.Parse(HttpContext.Session.GetString("userId"));
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ReceiveOrderList));
        }

        public async Task<IActionResult> Delivered(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.StatusId = 6;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ReceiveOrderList));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int userId;
            string roleName = HttpContext.Session.GetString("roleName");
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (!isNum)
            {
                return RedirectToAction("SignIn", "Customer", new { area = "Default" });
            }
            if (roleName != "Shipper")
            {
                return RedirectToAction("Index", "Home", new { area = roleName });
            }
            ViewBag.username = _context.Users.Where(n => n.UserId == userId).FirstOrDefault().UserName;
            User user = await _context.Users
               .Include(u => u.Role)
               .FirstOrDefaultAsync(m => m.UserId == userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(IFormFile Avatar, [Bind("UserId,UserName,IdCard,Email,Phone,IsEmailActive,Password,RoleId,Address,Avatar,Date,IsDeleted")] User user)
        {
            int userId;
            bool isNum = int.TryParse(HttpContext.Session.GetString("userId"), out userId);
            if (ModelState.IsValid)
            {
                try
                {
                    if (Avatar != null)
                    {
                        user.Avatar = Avatar.FileName;
                        var uploadDirectory = Path.Combine(_environment.WebRootPath, "upload", "images", "avatars", "customer");
                        if (!Directory.Exists(uploadDirectory))
                        {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        var path = Path.Combine(uploadDirectory, Avatar.FileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await Avatar.CopyToAsync(fileStream);
                        if (userId.ToString() == HttpContext.Session.GetString("userId"))
                        {
                            HttpContext.Session.SetString("avatar", Avatar.FileName);
                        }
                    }
                    else
                    {
                        user.Avatar = _context.Users.AsNoTracking().FirstOrDefault(n => n.UserId == userId).Avatar;
                    }
                    foreach (PropertyInfo pi in user.GetType().GetProperties())
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(user);
                            if (string.IsNullOrEmpty(value))
                            {
                                string roleName = HttpContext.Session.GetString("roleName");
                                if (!isNum)
                                {
                                    return RedirectToAction("SignIn", "Customer", new { area = "Default" });
                                }
                                if (roleName != "Customer")
                                {
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
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
                                user = _context.Users.AsNoTracking().FirstOrDefault(m => m.UserId == userId);
                                ViewBag.mess = "Vui lòng điển đẩy đủ thông tin";
                                return View(user);
                            }
                        }
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserId == user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(user);
        }
    }
}
