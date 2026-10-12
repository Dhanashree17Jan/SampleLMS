using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Schoolnet.Domain;

namespace Schoolnet.WEBUI.Controllers;
public sealed class AccountController(ITrainingRepository repo,PasswordHasher<string> hasher) : Controller
{
    [AllowAnonymous] public IActionResult Login(string? returnUrl=null) { ViewBag.ReturnUrl=returnUrl; return View(); }
    [AllowAnonymous,HttpPost,EnableRateLimiting("login")]
    public async Task<IActionResult> Login(string username,string password,string? returnUrl=null)
    {
        var user=await repo.One("SELECT * FROM schoolnet.app_user WHERE username=@username AND active",("username",(username??"").Trim().ToLowerInvariant()));
        if(user is null || hasher.VerifyHashedPassword(user.Text("username"),user.Text("password_hash"),password??"")==PasswordVerificationResult.Failed)
        { ViewBag.Error="The username or password is incorrect."; ViewBag.ReturnUrl=returnUrl; return View(); }
        var claims=new[] { new Claim(ClaimTypes.NameIdentifier,user.Id().ToString()),new Claim(ClaimTypes.Name,user.Text("name")),new Claim(ClaimTypes.Role,user.Text("role")),new Claim("user_version",user.Text("updated_at")) };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));
        return LocalRedirect(Url.IsLocalUrl(returnUrl)?returnUrl!:"/");
    }
    [Authorize,HttpPost] public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return RedirectToAction(nameof(Login)); }
    [AllowAnonymous] public IActionResult Denied() { Response.StatusCode=403; return View(); }
}
