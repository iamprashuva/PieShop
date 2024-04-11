using Microsoft.EntityFrameworkCore;

namespace PieShop.Models
{
    public class PieShopDbContext: DbContext
    {
       public PieShopDbContext(DbContextOptions<PieShopDbContext> options) : base(options)
        {
        }

        public DbSet<Pie> pies { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ShoppingCartItem> shoppingCartItems { get; set; }
    }
}
