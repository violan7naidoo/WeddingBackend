using Microsoft.AspNetCore.Identity;
using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseSeeder(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        _context.Database.Migrate();
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[] { "Admin", "Family", "Guest" })
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _userManager.FindByEmailAsync("admin@ourbigday.com") != null) return;

        var admin = new ApplicationUser
        {
            UserName = "admin@ourbigday.com",
            Email = "admin@ourbigday.com",
            DisplayName = "Admin",
        };
        await _userManager.CreateAsync(admin, "Admin123!");
        await _userManager.AddToRoleAsync(admin, "Admin");
    }
}
