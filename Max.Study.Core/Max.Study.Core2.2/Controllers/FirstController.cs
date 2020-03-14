using System;
using Max.Core.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Max.Study.Core2.Controllers
{
    public class FirstController : Controller
    {
        // GET: /<controller>/
        //[Route("Home/Test")]  // 支持特性路由  
        public IActionResult Index(int? id)
        {
            base.ViewData["User1"] = new CurrentUser()
            {
                Id = 7,
                Name = "Y",
                Account = " ╰つ Ｈ ♥. 花心胡萝卜",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.ViewData["Something"] = 12345;

            base.ViewBag.Name = "Richard";
            base.ViewBag.Description = "Teacher";
            base.ViewBag.User = new CurrentUser()
            {
                Id = 7,
                Name = "IOC",
                Account = "限量版",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.TempData["User"] = new CurrentUser()
            {
                Id = 7,
                Name = "CSS",
                Account = "季雨林",
                Email = "KOKE",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };//后台可以跨action  基于session

            if (id == null)
            { 
                return this.Redirect("~/First/TempDataPage"); // 作为一个待处理项
            }

            else
                return View(new CurrentUser()
                {
                    Id = 7,
                    Name = "一点半",
                    Account = "季雨林",
                    Email = "KOKE",
                    Password = "落单的候鸟",
                    LoginTime = DateTime.Now
                });
        }

        public IActionResult TempDataPage()
        {
            base.ViewBag.User = base.TempData["User"];//可以拿到数据
            return View();
        }
    }
}