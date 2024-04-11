
namespace PieShop.Models
{
    public class ShoppingCart:IShoppingCart
    {
        private readonly PieShopDbContext _pieShopDbContext;
        public string? ShoppingCartId { get; set; }

        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;
        private ShoppingCart(PieShopDbContext pieShopDbContext, string? shoppingCartId, List<ShoppingCartItem> shoppingCartItems)
        {
            _pieShopDbContext = pieShopDbContext;
        }


        public void AddToCart(Pie pie)
        {
            throw new NotImplementedException();
        }

        public void ClearCart()
        {
            throw new NotImplementedException();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            throw new NotImplementedException();
        }

        public decimal GetShoppingCartTotal()
        {
            throw new NotImplementedException();
        }

        public int RemoveFromCart(Pie pie)
        {
            throw new NotImplementedException();
        }
    }
}
