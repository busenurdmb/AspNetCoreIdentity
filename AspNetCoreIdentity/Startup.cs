using AspNetCoreIdentity.Context;
using AspNetCoreIdentity.CustomDescriber;
using AspNetCoreIdentity.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddIdentity<AppUser, AppRole>(opt=>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 1;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                //opt.SignIn.RequireConfirmedEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
            }).AddErrorDescriber<CustomErrorDescriber>().AddEntityFrameworkStores<IdentityContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                //httponly=true olursa ilgili Cookie javascript arac�l����yla �ekilemiyor
                opt.Cookie.HttpOnly = true;
                //Samsite modunu strict ayaralarsam sadece ilgili domiende kullan�labilir.
                opt.Cookie.SameSite = SameSiteMode.Strict;
                //alwayse derseniz sade http te �al��s�n sameis request httpyle
                //gelirse httpde https ie gelirse onda
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.Cookie.Name = "IdentityCookie";
                opt.ExpireTimeSpan = TimeSpan.FromDays(25);
                opt.LoginPath = new PathString("/Home/SignIn");
                opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
            });
            services.AddDbContext<IdentityContext>(opt =>
            {
                opt.UseSqlServer("server=DESKTOP-493DFJA\\SQLEXPRESS;database=IdentityDB;integrated security=true");
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = "/node_modules",
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules"))


            }); 
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                
            });
        }
    }
}
