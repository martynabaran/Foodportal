using Microsoft.EntityFrameworkCore;
using Foodbook.Data;
using Foodbook.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor(); 

// register db

builder.Services.AddDbContext<FoodbookDBContext>(options => options.UseSqlite("Data Source=Foodbook.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout value
    options.Cookie.HttpOnly = true; // Set cookie as essential
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope()) {
    // create new user if database is empty
    using(FoodbookDBContext ?db = scope.ServiceProvider.GetService<FoodbookDBContext>()) {
        if (db is null) {
            throw new Exception("UserDBContext is null");
        }

        if (db.Users.Count() == 0) {
            String password = "admin";
            String salt = Foodbook.Controllers.CryptoCalculator.GenerateRandomString(128);

            db.Users.Add(new UserModel {
                Username = "admin",
                PasswordHash = Foodbook.Controllers.CryptoCalculator.CreateSHA256WithSalt(password, salt),
                PasswordSalt = salt,
                IsAdmin = true,
                IsApproved = true
            });

            db.SaveChanges();
        }
    }
}

app.Run();
// using Microsoft.EntityFrameworkCore;
// using Microsoft.OpenApi.Models;
// using Foodbook.Data;
// using Microsoft.AspNetCore.Http;
// using Foodbook.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();
//  builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "YourApi", Version = "v1" });
// });

// // Dodaj kontekst bazy danych
// builder.Services.AddDbContext<FoodbookDBContext>(options => options.UseSqlite("Data Source=Foodbook.db"));

// // Dodaj sesje
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });

// // Dodaj dostęp do HTTP context
// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// // Konfiguracja CORS (jeśli konieczne)
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll",
//         builder => builder.AllowAnyOrigin()
//                           .AllowAnyMethod()
//                           .AllowAnyHeader());
// });

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YourApi v1"));
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// // Dodaj obsługę sesji
// app.UseSession();

// // Konfiguracja CORS (jeśli konieczne)
// app.UseCors("AllowAll");

//  app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");


// using (var scope = app.Services.CreateScope()) {
//     // create new user if database is empty
//     using(FoodbookDBContext ?db = scope.ServiceProvider.GetService<FoodbookDBContext>()) {
//         if (db is null) {
//             throw new Exception("UserDBContext is null");
//         }

//         if (db.Users.Count() == 0) {
//             String password = "admin";
//             String salt = Foodbook.Controllers.CryptoCalculator.GenerateRandomString(128);

//             db.Users.Add(new UserModel {
//                 Username = "admin",
//                 PasswordHash = Foodbook.Controllers.CryptoCalculator.CreateSHA256WithSalt(password, salt),
//                 PasswordSalt = salt,
//                 IsAdmin = true,
//                 IsApproved = true
//             });

//             db.SaveChanges();
//         }
//     }
// }
// app.Run();
