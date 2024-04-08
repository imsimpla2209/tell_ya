using DiaryManagement.Application.Services;
using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Data;
using DiaryManagement.Infrastructure.Extentions;
using DiaryManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiaryManagement.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;


                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/Authentication/Authentication/Login";
                options.LogoutPath = "/Authentication/Authentication/Logout";
                options.SlidingExpiration = true;
            });

            builder.Services.AddAuthentication()
              .AddGoogle(googleOptions =>
              {
                  googleOptions.ClientId = config["GoogleAuthentication:ClientID"];
                  googleOptions.ClientSecret = config["GoogleAuthentication:ClientSecret"];
              });

            //Service register
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IWriterService, WriterService>();
            builder.Services.AddScoped<IDiaryService, DiaryService>();
            //Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IWriterRepository, WriterRepository>();
            builder.Services.AddScoped<IDiaryRepository, DiaryRepository>();

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Add logging
            builder.Services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            SeedingHelper.SeedData(builder.Services.BuildServiceProvider());
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                 name: "AuthArea",
                 pattern: "{area:exists}/{controller=Authentication}/{action=Login}");
            app.MapControllerRoute(
                 name: "AdminArea",
                 pattern: "{area:exists}/{controller=Admin}/{action=Index}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}