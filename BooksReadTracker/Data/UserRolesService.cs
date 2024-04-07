using Microsoft.AspNetCore.Identity;

namespace BooksReadTracker.Data
{
    public class UserRolesService : IUserRolesService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public const string ADMIN_ROLE_NAME = "Admin";
        private const string ADMIN_USER_EMAIL = "starbuck@galactica.com";
        private const string ADMIN_USER_PASSWORD = "frakTheCylons#1!";

        public UserRolesService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureUsersAndRoles()
        {
            await EnsureAdminUserRole();
        }

        private async Task EnsureAdminRole()
        {

            var roleExists = await _roleManager.RoleExistsAsync(ADMIN_ROLE_NAME);
            if (!roleExists)
            {
                var adminRole = new IdentityRole()
                {
                    Name = ADMIN_ROLE_NAME,
                    NormalizedName = ADMIN_ROLE_NAME.ToUpper()
                };
                await _roleManager.CreateAsync(adminRole);
            }
        }

        private async Task EnsureAdminUser()
        {
            var existingAdminUser = await _userManager.FindByEmailAsync(ADMIN_USER_EMAIL);
            if (existingAdminUser is null)
            {
                var adminUser = new IdentityUser()
                {
                    Email = ADMIN_USER_EMAIL,
                    EmailConfirmed = true,
                    UserName = ADMIN_USER_EMAIL,
                    NormalizedEmail = ADMIN_USER_EMAIL.ToUpper(),
                    NormalizedUserName = ADMIN_USER_EMAIL.ToUpper(),
                    LockoutEnabled = false
                };

                await _userManager.CreateAsync(adminUser, ADMIN_USER_PASSWORD);
            }
        }

        private async Task EnsureAdminUserRole()
        {
            //ensure roles
            await EnsureAdminRole();

            //ensure users
            await EnsureAdminUser();

            var existingAdminUser = await _userManager.FindByEmailAsync(ADMIN_USER_EMAIL);
            var existingRole = await _roleManager.FindByNameAsync(ADMIN_ROLE_NAME);
            if (existingAdminUser is null || existingRole is null)
            {
                throw new InvalidOperationException("Cannot add  null user/role combination");
            }

            var userRoles = await _userManager.GetRolesAsync(existingAdminUser);
            var existingUserAdminRole = userRoles.SingleOrDefault(x => x.Equals(ADMIN_ROLE_NAME));

            if (existingUserAdminRole is null)
            {
                await _userManager.AddToRoleAsync(existingAdminUser, ADMIN_ROLE_NAME);
            }
        }
    }
}
