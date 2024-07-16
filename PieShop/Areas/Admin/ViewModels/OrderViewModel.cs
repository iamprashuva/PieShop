using PieShop.Models;

namespace PieShop.Areas.Admin.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }

        public IEnumerable<Pie> Pies { get; set; }
    }
}

