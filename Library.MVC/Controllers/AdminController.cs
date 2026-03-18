using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.MVC.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(RoleManager<IdentityRole> roleManager) : Controller
{
    public IActionResult Roles() => View(roleManager.Roles.ToList());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName) && !await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        return RedirectToAction(nameof(Roles));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is not null) await roleManager.DeleteAsync(role);
        return RedirectToAction(nameof(Roles));
    }
}