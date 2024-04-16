
using Microsoft.EntityFrameworkCore;

namespace PieShop.Models
{
    public class ShoppingCart:IShoppingCart
    {
        private readonly PieShopDbContext _pieShopDbContext;
        public string? ShoppingCartId { get; set; }

        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;
        private ShoppingCart(PieShopDbContext pieShopDbContext)
        {
            _pieShopDbContext = pieShopDbContext;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            //Access to the session
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            //Access to the DbContext
            PieShopDbContext context = services.GetService<PieShopDbContext>() ?? throw new Exception("Erroe Initializing");
            //Check for the session containing value CartId and if not then create a new one
            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            //Set value of CartId 
            session?.SetString("CartId", cartId);
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }
        public void AddToCart(Pie pie)
        {
            //If the items available already
            var shoppingCartItem = _pieShopDbContext.ShoppingCartItems.SingleOrDefault(
                s => s.Pie.PieId == pie.PieId && s.ShoppingCartId ==
                ShoppingCartId);
            if(shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 1
                };
                _pieShopDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _pieShopDbContext.SaveChanges();

        }

        public void ClearCart()
        {
            throw new NotImplementedException();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??=
                _pieShopDbContext.ShoppingCartItems.Where(c => 
                c.ShoppingCartId == ShoppingCartId)
                .Include(s => s.Pie)
                .ToList();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _pieShopDbContext.ShoppingCartItems.Where(c=>
            c.ShoppingCartId == ShoppingCartId)
                .Select(c=>
                c.Pie.Price * c.Amount).Sum();
            return total;
        }

        public int RemoveFromCart(Pie pie)
        {
            //If the items available already
            var shoppingCartItem = _pieShopDbContext.ShoppingCartItems.SingleOrDefault(
                s => s.Pie.PieId == pie.PieId && s.ShoppingCartId ==
                ShoppingCartId);
            var localAmount = 0;

            if(shoppingCartItem != null)
            {
                if(shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _pieShopDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _pieShopDbContext.SaveChanges();
            return localAmount;

        }
    }
}
