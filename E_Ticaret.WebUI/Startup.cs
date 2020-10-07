using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.Business.Concrete;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.DataAccess.Concrete.EfCore;
using E_Ticaret.WebUI.EmailServices;
using E_Ticaret.WebUI.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace E_Ticaret.WebUI
{
    public class Startup
    {
        public IConfiguration _configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("IdentityConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //password 
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            });


            services.ConfigureApplicationCookie(options =>
            {

                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";              
                options.AccessDeniedPath = "/Account/Accessdenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = "E_Ticaret.Security.Cookie"

                };
            });

            services.AddScoped<IProductDal, EfCoreProductDal>();
            services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
            services.AddScoped<ICartDal, EfCoreCartDal>();
            services.AddScoped<IOrderDal, EfCoreOrderDal>();


            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICartService, CartManager>();
            services.AddScoped<IOrderService, OrderManager>();



            services.AddTransient<IEmailSender, EmailSender>();//Mail Servisi


            services.AddMvc();

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //SeedDatabase.Seed();
            }


            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(name: "adminProducts",  
                pattern: "admin/products",
                defaults: new { controller = "Admin", action = "ListProduct" });

                endpoints.MapControllerRoute(name: "adminProducts",  
                pattern: "admin/products/{id?}",
                defaults: new { controller = "Admin", action = "EditProduct" });

                endpoints.MapControllerRoute(name: "cart",  
                pattern: "cart",
                defaults: new { controller = "Cart", action = "Index" });

                endpoints.MapControllerRoute(name: "checkout", 
                pattern: "checkout",
                defaults: new { controller = "Cart", action = "Checkout" });

                endpoints.MapControllerRoute(name: "orders",  
                pattern: "orders",
                defaults: new { controller = "Cart", action = "GetOrders" });

                endpoints.MapControllerRoute(name: "products",  
            pattern: "products/{*category}",
            defaults: new { controller = "Shop", action = "List" });

                endpoints.MapControllerRoute(name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
