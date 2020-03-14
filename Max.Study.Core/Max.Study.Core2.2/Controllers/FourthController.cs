using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Study.Core2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Interface;
using Max.Study.Core2.Utility;
using Max.Core.Utility.Filters;
using Microsoft.AspNetCore.Authentication;
using Max.Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Max.Study.Core2.Controllers
{

    /// <summary>
    /// log4Net 在控制器中写日志 
    /// core 内置了容器
    /// </summary>
  /*  [Authorize]*/ // 如果说在标记权限特性的时候 不指定任何验证方式，那么会三种验证方式同时验证
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    //[TypeFilter(typeof(CustomControllerActionFilterAttribute), Order = -1)]
    public class FourthController : Controller
    {
        private ILoggerFactory _factory = null;
        private ILogger<SecondController> _ilogger = null;

        private ITestServiceA _testServiceA = null;
        private ITestServiceB _testServiceB = null;
        private ITestServiceC _testServiceC = null;
        private ITestServiceD _testServiceD = null;
        private IA _a = null;

        public FourthController(ILoggerFactory factory, ILogger<SecondController> ilogger,
          ITestServiceA testServiceA,
          ITestServiceB testServiceB,
          ITestServiceC testServiceC,
          ITestServiceD testServiceD,
          IA a)
        {
            _factory = factory;
            _ilogger = ilogger;
            _testServiceA = testServiceA;
            _testServiceB = testServiceB;
            _testServiceC = testServiceC;
            _testServiceD = testServiceD;
            _a = a;
        }



        //[Authorize(Policy = "Advanced")]
        // 表是基于Sechme的验证
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Policy = "AdvancedRequirement")] // 基于策略验证
        //[TypeFilter(typeof(CustomAuthorizeActionFilterAttribute))]

         
        public IActionResult Index()
        {
            // 其实在这里就可以做一些基于 HttpContext?.User 权限的验证了 
            var userName = HttpContext?.User?.Identity?.Name;
            return View();
        }

        [CutomAllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        //[Authorize]
        [CutomAllowAnonymous]
        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            this._ilogger.LogDebug($"{name} {password} 登陆系统");
            #region 这里应该是要到数据库中查询验证的
            CurrentUser currentUser = new CurrentUser()
            {
                Id = 123,
                Name = "Richard",
                Account = "Administrator",
                Password = "123456",
                Email = "18672713698@163.com",
                LoginTime = DateTime.Now,
                Role = name.Equals("Richard") ? "Admin" : "User"
            };
            #endregion

            #region session+filter
            //base.HttpContext.Session.SetString("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser));
            #endregion

            #region cookie
            {
                ////就很像一个CurrentUser,转成一个claimIdentity
                var claimIdentity = new ClaimsIdentity("Cookie");
                claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, currentUser.Name));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, currentUser.Email));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, currentUser.Role));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Sid, currentUser.Id.ToString()));
                var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                // 在上面注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。 
                // 这里的SignIn不是MyHandler 里的SignIn
                base.HttpContext.SignInAsync(claimsPrincipal).Wait();//不就是写到cookie
            }
            #endregion

            return View();
        }

        [CutomAllowAnonymous]
        public ActionResult Logout()
        {
            base.HttpContext.SignOutAsync().Wait();
            return this.Redirect("~/Fourth/Login");
        }


    }
}