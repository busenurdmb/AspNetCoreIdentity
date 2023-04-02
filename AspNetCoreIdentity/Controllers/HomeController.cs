using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        //user ı Create ederken

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;



        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult AccessDenied()
        {

            return View();
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel p)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Email = p.Email,
                    Gender = p.Gender,
                    UserName = p.Username
                };


                var identityResult = await _userManager.CreateAsync(user, p.Password);
                if (identityResult.Succeeded)
                {
                 var memberRole=await _roleManager.FindByNameAsync("Member");
                    if (memberRole == null)
                    {
                    await _roleManager.CreateAsync(new()
                    {
                        Name = "Member",
                        CreatTime = DateTime.Now,
                    });
                    }
                    
                    await _userManager.AddToRoleAsync(user, "Member");
                    return RedirectToAction("Index");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                //business
            }
            return View(p);
        }

        public IActionResult SignIn(string returnUrl)
        {
           
            return View(new UserSignInModel() { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel p)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(p.Username);
                var signInResult = await _signInManager.PasswordSignInAsync(p.Username, p.Password, p.RememberMe, true);
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(p.ReturnUrl))
                    {
                        return Redirect(p.ReturnUrl);
                    }

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminPanel");
                    }
                    else
                    {
                        return RedirectToAction("Panel");
                    }
                }
                else if (signInResult.IsLockedOut)
                {
                    var lockOutEnd= await _userManager.GetLockoutEndDateAsync(user);
                   
                    
                    //hesap kilitli
                    ModelState.AddModelError("",$"Hesabınız {(lockOutEnd.Value.UtcDateTime-DateTime.UtcNow).Minutes} dk askıya alınmıştır");
                }
                else
                {
                    var message = string.Empty;

                    if (user != null)
                    {
                        //userr ın kaç kere hatalı girdiğine bakıyoruz
                        var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        message = $"{(_userManager.Options.Lockout.MaxFailedAccessAttempts - failedCount)} kez daha girerseniz heasabınız geçici olarak kilitlenicektir  ";
                    }
                    else
                    {
                        message = "kullanıcı adı veya şifre hatalı";
                    }
                    ModelState.AddModelError("", message);

                }
                //else if (signInResult.IsNotAllowed)
                //{
                //    //email&&phonenumber doğrulanmamış
                //}
            }
            return View(p);
        }
        //sadece giriş yapmş kullanıcılar diyoz
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
            ViewBag.ad = userName;
            ViewBag.role = role;//bu role sahipmi diye soruyoz
            User.IsInRole("Memmber");
            return View();
        }
        [Authorize(Roles="Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult Panel()
        {
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult MemberPage()
        {
            return View();
        }
        public async  Task<IActionResult>  LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
