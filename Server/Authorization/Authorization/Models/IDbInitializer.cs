using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Models
{
    public interface IDbInitializer
    {
        void Initialize();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //This example just creates an Administrator role and one Admin users
        public async void Initialize()
        {
            //create database schema if none exists
            _context.Database.EnsureCreated();

            //If there is already an Administrator role, abort
            if (!_context.Roles.Any(r => r.Name == "Administrator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            //Create the default Admin account and apply the Administrator role
            var user = "test@test.com";
            var password = "Test_0";

            if (!_context.Users.Any(x => x.Email.Equals(user)))
            {
                var x = await _userManager.CreateAsync(new User { UserName = user, Email = user, EmailConfirmed = true }, password);
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
            }


        }
    }
}