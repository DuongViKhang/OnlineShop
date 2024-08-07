﻿using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string IdCard { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int IsEmailActive { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? Date { get; set; }
        public int IsDeleted { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
