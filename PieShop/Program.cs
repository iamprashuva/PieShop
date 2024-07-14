using Microsoft.EntityFrameworkCore;
using PieShop.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


//Register DbContext
var ConnectionStrings = builder.Configuration.GetConnectionString("PieShop");
builder.Services.AddDbContext<PieShopDbContext>(options =>
{
    options.UseSqlServer(ConnectionStrings,
         sqlServerOptionsAction: sqlOptions =>
         {
             sqlOptions.EnableRetryOnFailure(
                 maxRetryCount: 5, // Adjust the maximum number of retries as needed
                 maxRetryDelay: TimeSpan.FromSeconds(30), // Adjust the maximum delay between retries as needed
                 errorNumbersToAdd: null);
         });
});

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<PieShopDbContext>();
//Invoke the GetCard method
builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
var app = builder.Build();

app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
}
app.UseSession();
// Set Stripe API key
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();


DbInitializer.Seed(app);
app.MapRazorPages();

app.Run();
