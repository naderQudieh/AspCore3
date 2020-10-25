using Microsoft.EntityFrameworkCore;
using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;

namespace ASP.NETCoreAuthWebApi.DataAcces
{
    public class CartContext : DbContext
    {
        public CartContext(DbContextOptions<CartContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Product> Cart { get; set; }
    }
}
