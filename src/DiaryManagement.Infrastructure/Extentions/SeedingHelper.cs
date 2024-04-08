using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Constants;
using DiaryManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DiaryManagement.Infrastructure.Extentions
{
    public static class SeedingHelper
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
            {
                if (!roleManager.Roles.Any())
                {
                    string[] roleNames = { RoleName.Manager, RoleName.Member };

                    foreach (var roleName in roleNames)
                    {
                        var role = new IdentityRole(roleName);
                        roleManager.CreateAsync(role).Wait();
                    }
                }
            }

            using (var userManager = serviceProvider.GetRequiredService<UserManager<User>>())
            {
                if (!userManager.Users.Any())
                {
                    User admin = new User()
                    {
                        FirstName = "Admin",
                        LastName = "Blogger",
                        UserName = "realblogger@gmail.com",
                        Email = "realblogger@gmail.com",
                        IsActivated = true,
                    };
                    User user = new User()
                    {
                        FirstName = "User",
                        LastName = "Member",
                        UserName = "user_member",
                        Email = "user_member@gmail.com",
                        IsActivated = true,
                    };
                    var result = userManager.CreateAsync(admin, "Realblogger#1").Result;
                    var result1 = userManager.CreateAsync(user, "Member#1").Result;
                    userManager.AddToRoleAsync(admin, RoleName.Manager).Wait();
                    userManager.AddToRoleAsync(user, RoleName.Member).Wait();
                }
            }

            using (var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                if (!dbContext.Categories.Any())
                {
                    dbContext.Categories.AddRange(
                        new Category()
                        {
                            CategoryId = Guid.NewGuid().ToString(),
                            CategoryName = "Secret Blog"
                        },
                        new Category()
                        {
                            CategoryId = Guid.NewGuid().ToString(),
                            CategoryName = "Niche Blog"
                        },
                        new Category()
                        {
                            CategoryId = Guid.NewGuid().ToString(),
                            CategoryName = "Social Blog"
                        },
                        new Category()
                        {
                            CategoryId = Guid.NewGuid().ToString(),
                            CategoryName = "Daily Blog"
                        },
                        new Category()
                        {
                            CategoryId = Guid.NewGuid().ToString(),
                            CategoryName = "Technology Blog"
                        }
                    );
                    dbContext.SaveChanges();
                }

                if (!dbContext.Writers.Any())
                {
                    dbContext.Writers.AddRange(
                        new Writer()
                        {
                            WriterId = Guid.NewGuid().ToString(),
                            WriterName = "Nguyễn Văn Yeah"
                        },
                        new Writer()
                        {
                            WriterId = Guid.NewGuid().ToString(),
                            WriterName = "Lê Liems Bảy"
                        },
                        new Writer()
                        {
                            WriterId = Guid.NewGuid().ToString(),
                            WriterName = "Đặng Phong Mười"
                        }
                    );
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
