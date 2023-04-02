using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

      
        public IActionResult Index()
        {
           return View();
        }
        public IActionResult list()
        {
            var list = _roleManager.Roles.ToList();
            return View(list);
        }
        public IActionResult CreateRole()
        {
            return View(new RoleCreateModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateModel p)
        {
            if (ModelState.IsValid)
            {
                var role = new AppRole
                {
                    Name = p.Name,
                    CreatTime = DateTime.Now
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("list");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
    }
}
