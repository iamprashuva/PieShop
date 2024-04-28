namespace PieShop.Models
{
    public class OrderRepository: IOrderRepository
    {
        private readonly PieShopDbContext _pieShopDbContext;
        private readonly IShoppingCart _shoppingCart;

        public OrderRepository(PieShopDbContext pieShopDbContext, IShoppingCart shoppingCart)
        {
            _pieShopDbContext = pieShopDbContext;
            _shoppingCart = shoppingCart;
        }

        public void CreateOrder(Order order)
        {
            order.OrderPlaced = DateTime.Now;

            List<ShoppingCartItem>? shoppingCartItems = _shoppingCart.ShoppingCartItems;
            order.OrderTotal = _shoppingCart.GetShoppingCartTotal();

            order.OrderDetails = new List<OrderDetail>();

            foreach (ShoppingCartItem? shoppingCartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Amount = shoppingCartItem.Amount,
                    PieId = shoppingCartItem.Pie.PieId,
                    Price = shoppingCartItem.Pie.Price
                };

                order.OrderDetails.Add(orderDetail);
            }

            _pieShopDbContext.Orders.Add(order);

            _pieShopDbContext.SaveChanges();
        }
    }
}
