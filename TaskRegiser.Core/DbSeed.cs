using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskRegiser.Core.Entities;

namespace TaskRegiser.Core
{
    public static class DbSeed
    {
        private static IServiceProvider _serviceProvider;
        public static async Task InitializeAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            await SeedUsersRolesAsync();
        }

        private static async Task SeedUsersRolesAsync()
        {
            var RoleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleTitles = { "Admin", "Member" };
            IdentityResult roleResult;
            foreach(var roleName in roleTitles)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if(!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var UserManager = _serviceProvider.GetRequiredService<UserManager<Employee>>();

           
            string[] logins = { "Admin@TestMail.com" };
            var userExist = await UserManager.FindByNameAsync(logins[0]);
            if(userExist==null)
            {
                var User = new Employee
                {
                    Email = "Admin@TestMail.com",
                    UserName = logins[0]
                };
                await UserManager.CreateAsync(User, "1qazZAQ!"); //Admin1 as password is a joke :D 
                await UserManager.AddToRoleAsync(User, roleTitles[0]);
            }
        }
    }
}
