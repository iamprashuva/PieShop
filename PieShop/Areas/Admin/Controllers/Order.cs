using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PieShop.Areas.Admin.ViewModels;
using PieShop.Models;

namespace PieShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Order : Controller
    {
     
          private readonly PieShopDbContext _context;

        public Order(PieShopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/OrderDetails
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.ToListAsync();
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Pie)
            .ToListAsync();
            var pies = await _context.pies.ToListAsync();

            var viewModel = new OrderViewModel
            {
                Orders = orders,
                OrderDetails = orderDetails,
                Pies = pies
            };

            return View(viewModel);
        }


    }
}

