using Microsoft.EntityFrameworkCore;
using PieShop.Models;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("PieShop");
builder.Services.AddDbContext<PieShopDbContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
});

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PieShopDbContext>();

builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddMemoryCache();
builder.Services.AddMvc();

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
using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager","Member" };
    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "prasuva@gmail.com";
    string password = "P@ssW0rd";
    if (await userManager.FindByEmailAsync(email)==null)
    {
        var user = new IdentityUser();
        user.UserName = email;
        user.Email = email;
        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user,"Admin");

    }
    }



app.Run();
