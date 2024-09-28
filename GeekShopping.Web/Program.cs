using GeekShopping.Web.Services;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<IProductService, ProductService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrl:ProductAPI"])
    );

builder.Services.AddHttpClient<ICartService, CartService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrl:CartAPI"])
    );
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "Cookies";
    opt.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", opt =>
    {
        opt.Authority = builder.Configuration["ServiceUrl:IdentityServer"];
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.ClientId = "geek_shopping";
        opt.ClientSecret = "my_super_secret";
        opt.ResponseType = "code";
        opt.ClaimActions.MapJsonKey("role", "role", "role");
        opt.ClaimActions.MapJsonKey("sub", "sub", "sub");
        opt.TokenValidationParameters.NameClaimType = "name";
        opt.TokenValidationParameters.RoleClaimType = "role";
        opt.Scope.Add("geek_shopping");
        opt.SaveTokens = true;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
