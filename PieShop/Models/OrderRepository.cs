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

                // Save order to database
                _pieShopDbContext.Orders.Add(order);
                _pieShopDbContext.SaveChanges();
            
        }
    }
}
