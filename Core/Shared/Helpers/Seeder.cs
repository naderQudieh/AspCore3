using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using AppZeroAPI.Entities;
using AppZeroAPI.Shared;

namespace AppZeroAPI.Shared
{
    public static class SeedDB
    {
        public static List<UserProfile> getUsers()
        {


            List<UserProfile> users = new List<UserProfile> {
                new UserProfile { user_id =  1, username = "admin01", email =  "admin01@gmail.com", role = Role.Admin, date_created = DateTime.Now, date_modified = DateTime.Now },
                new UserProfile { user_id =  2, username = "member01", email =  "member01@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
                new UserProfile { user_id =  3, username = "member02", email =  "member02@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
                new UserProfile { user_id =  4, username = "member03", email =  "member03@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new  UserProfile { user_id =  5, username = "member04", email =  "member04@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new UserProfile { user_id =  6, username = "member05", email =  "member05@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new UserProfile { user_id =  7, username = "member06", email =  "member06@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new UserProfile { user_id =  8, username = "member07", email =  "member07@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new  UserProfile { user_id =  9, username = "member08", email =  "member08@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new UserProfile { user_id =  10, username = "member09", email =  "member09@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now },
               new UserProfile { user_id =  11, username = "member10", email =  "member10@gmail.com", role = Role.Member, date_created = DateTime.Now, date_modified = DateTime.Now }
            };
            return users;
        }
        public static List<CustomerCart> getCarts()
        {
            List<CustomerCart> customerCarts = new List<CustomerCart> {
                new CustomerCart {rec_id =  "1", customer_id =  "1", cart_total = 3, total_payable = 12,   date_created = DateTime.Now, date_modified = DateTime.Now ,
                 cartItems = new List<CartItem> {
                  new CartItem {rec_id =  "1", cart_id =  "1", product_id =  "1", qty = 12, total_payable =  215,  date_created = DateTime.Now, date_modified = DateTime.Now },
                  new CartItem {rec_id =  "2", cart_id =  "1", product_id =  "2", qty = 22, total_payable =  215,  date_created = DateTime.Now, date_modified = DateTime.Now }
                 }
                 },
                new CustomerCart {rec_id =  "2", customer_id =  "2", cart_total = 2, total_payable =  15,  date_created = DateTime.Now, date_modified = DateTime.Now,
                 cartItems = new List<CartItem> {
                  new CartItem {rec_id =  "1", cart_id =  "2", product_id =  "1", qty = 12, total_payable =  215,  date_created = DateTime.Now, date_modified = DateTime.Now },
                  new CartItem {rec_id =  "2", cart_id =  "2", product_id =  "2", qty = 22, total_payable =  215,  date_created = DateTime.Now, date_modified = DateTime.Now }
                 }
                },
             };
            return customerCarts;
        }

    }
}