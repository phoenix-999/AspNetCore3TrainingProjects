using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAppFromEmpty.Data;
using IdentityAppFromEmpty.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAppFromEmpty.Contorllers
{
    [Authorize]
    public class AccountController : Controller
    {
        IdentityDbContext db;

        public AccountController(IdentityDbContext context)
        {
            db = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Register(RegisterViewModel model)
        //{

        //}
    }
}