using Microsoft.AspNetCore.Identity;

public static class IdentitySeed
{
  public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
  {
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    string[] roles = { "Admin", "Manager", "User" };

    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
      }
    }
  }
}