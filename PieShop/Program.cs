using Microsoft.EntityFrameworkCore;
using PieShop.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();

//Register DbContext
var ConnectionStrings = builder.Configuration.GetConnectionString("PieShop");
builder.Services.AddDbContext<PieShopDbContext>(options =>
{
    options.UseSqlServer(ConnectionStrings);
});
//Invoke the GetCard method
builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
}

app.UseSession();
app.MapDefaultControllerRoute();


DbInitializer.Seed(app);

app.Run();
