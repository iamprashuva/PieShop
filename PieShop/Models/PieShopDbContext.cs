using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PieShop.Models
{
    public class PieShopDbContext: IdentityDbContext
    {
       public PieShopDbContext(DbContextOptions<PieShopDbContext> options) : base(options)
        {
        }

        public DbSet<Pie> pies { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
