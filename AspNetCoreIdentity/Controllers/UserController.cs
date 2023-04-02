using AspNetCoreIdentity.Context;
using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IdentityContext _ıdentityContext;
        private readonly RoleManager<AppRole> _roleManager;
        public UserController(UserManager<AppUser> userManager, IdentityContext ıdentityContext, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _ıdentityContext = ıdentityContext;
        }

        public  async  Task<IActionResult> Index()
        {
            //var query=  _userManager.Users;
            //  var users = _ıdentityContext.Users.Join(_ıdentityContext.UserRoles, user => user.Id, userrole => userrole.UserId, (user, userrole) => new
            //  {
            //      user,
            //      userrole
            //  }).Join(_ıdentityContext.Roles, two => two.userrole.RoleId, role => role.Id, (two, role) => new { two.user, two.userrole, role }).Where(x => x.role.Name != "Admin").Select(x => new AppUser
            //  {
            //      Id = x.user.Id,
            //      AccessFailedCount = x.user.AccessFailedCount,
            //      ConcurrencyStamp = x.user.ConcurrencyStamp,
            //      Email = x.user.Email,
            //      EmailConfirmed = x.user.EmailConfirmed,
            //      Gender = x.user.Gender,
            //      ImagePath = x.user.ImagePath,
            //      LockoutEnabled = x.user.LockoutEnabled,
            //      LockoutEnd = x.user.LockoutEnd,
            //      NormalizedEmail = x.user.NormalizedEmail,
            //      NormalizedUserName = x.user.NormalizedUserName,
            //      PasswordHash = x.user.PasswordHash,
            //      PhoneNumber = x.user.PhoneNumber,
            //      UserName = x.user.UserName,


            //  }).ToList();
            //var user = await _userManager.GetUsersInRoleAsync("Member");
            //return View(users);
            List<AppUser> filteredUser = new List<AppUser>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                    filteredUser.Add(user);
            }
            return View(filteredUser);
        }
        public IActionResult Create()
        {
            return View(new UserAdminCreateModel());
        }
        [HttpPost]
        public async  Task<IActionResult> Create(UserAdminCreateModel p)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = p.Email,
                    UserName = p.Username,
                    Gender = p.Gender,
                };
              var result=  await  _userManager.CreateAsync(user, p.Username + "123");
                if (result.Succeeded)
                {
                    var memberRole = await _roleManager.FindByNameAsync("Member");
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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return View(p);
        }
        public async Task<IActionResult> AssignRole(int id)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
            var userRoles =await  _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.ToList();

            RoleAssingSendModel model = new RoleAssingSendModel();
            List<RoleAssignListModel> list = new List<RoleAssignListModel>();
            foreach (var role in roles)
            {
                list.Add(new()
                {
                    Name = role.Name,
                    RoleId = role.Id,
                    Exist = userRoles.Contains(role.Name)
                });
            }
            model.Roles = list;
            model.UserId = id;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssingSendModel p)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.Id == p.UserId);

            var userRoles =await _userManager.GetRolesAsync(user);

            foreach (var role in p.Roles)
            {
                if (role.Exist)
                {
                    if (!userRoles.Contains(role.Name))
                     await   _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
