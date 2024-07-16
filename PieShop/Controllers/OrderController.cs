using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PieShop.Models;
using Stripe.Checkout;


namespace PieShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCart _shoppingCart;
        private readonly IConfiguration _configuration;

        public OrderController(IOrderRepository orderRepository, IShoppingCart shoppingCart, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckOut(Order order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some pies first");
            }
            if (ModelState.IsValid)
            {
                // Store order details temporarily
                TempData["OrderDetails"] = JsonConvert.SerializeObject(order);

                // Log for debugging
                System.Diagnostics.Debug.WriteLine("Redirecting to Payment action");

                return RedirectToAction("Payment", "Order");

            }
            // Log ModelState errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine("ModelState Error: " + error.ErrorMessage);
            }
            return View(order);
        }
        [HttpGet]
        public IActionResult CheckoutComplete()
        {
            var sessionId = TempData["Session"] as string;

            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("CheckOut");
            }

            var service = new SessionService();
            Session session = service.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                // Retrieve stored order details
                var orderJson = TempData["OrderDetails"] as string;
                if (string.IsNullOrEmpty(orderJson))
                {
                    // Handle error scenario (no order details found)
                    ModelState.AddModelError("", "No order details found, please provide order details first.");
                    return RedirectToAction("CheckOut");
                }

                var order = JsonConvert.DeserializeObject<Order>(orderJson);

                // Set order details
                order.OrderPlaced = DateTime.Now;
                order.OrderTotal = _shoppingCart.GetShoppingCartTotal();

                // Retrieve shopping cart items
                var shoppingCartItems = _shoppingCart.GetShoppingCartItems();

                // Create order details
                order.OrderDetails = shoppingCartItems.Select(item => new OrderDetail
                {
                    PieId = item.Pie.PieId,
                    Amount = item.Amount,
                    Price = item.Pie.Price
                }).ToList();

                // Save order to database
                _orderRepository.CreateOrder(order);

                // Clear temporary data
                TempData.Remove("OrderDetails");

                // Optionally clear session data or handle session cleanup

                return View();
            }

            // Handle scenario where payment status is not "paid"
            return RedirectToAction("CheckoutCancelled");
        }

        public IActionResult CheckoutCancelled()
        {
            ViewBag.CheckoutCancelledMessage = "Your payment was cancelled.";
            return View();
        }
    

    public IActionResult Payment()
        {
            var shoppingCartItems = _shoppingCart.GetShoppingCartItems();

            if (shoppingCartItems.Count == 0)
            {
                // Handle scenario where shopping cart is empty
                ModelState.AddModelError("", "Your cart is empty, add some pies first");
                return RedirectToAction("CheckOut");
            }

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in shoppingCartItems)
            {
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Pie.Price * 100), // Amount in cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Pie.Name
                        },
                    },
                    Quantity = item.Amount
                };
                lineItems.Add(lineItem);
            }

            var domain = "https://localhost:7002/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = domain + "Order/CheckoutComplete",
                CancelUrl = domain + "Order/CheckoutCancelled"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

    }
}
