
using Customer_manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Customer_manager.Repositories;


var builder = WebApplication.CreateBuilder(args);




builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<Customers, IdentityRole>(options =>
{
    // �ifre politikalar�n� ayarla
    options.Password.RequireDigit = false; // Rakam gereksinimi yok
    options.Password.RequiredLength = 3;   // Minimum �ifre uzunlu�u
    options.Password.RequireNonAlphanumeric = false; // Alfasay�sal olmayan karakter gereksinimi yok
    options.Password.RequireUppercase = false; // B�y�k harf gereksinimi yok
    options.Password.RequireLowercase = false; // K���k harf gereksinimi yok
    options.Password.RequiredUniqueChars = 1; // Minimum benzersiz karakter say�s�
    options.SignIn.RequireConfirmedAccount = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Yaln�zca harfler ve rakamlar

})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped(typeof(GenericRepository<>));

builder.Services.AddControllersWithViews();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
	
app.UseAuthorization();
app.UseAuthentication();


app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Register}/{action=Index}/{id?}");

app.Run();
