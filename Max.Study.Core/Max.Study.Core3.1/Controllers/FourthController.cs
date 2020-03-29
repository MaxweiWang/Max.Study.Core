using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Max.Core.Interface;
using Max.Study.Core3.Models;
using Max.Study.Core3.Utility;
using Max.EFCore3_1.Model;

namespace Max.Study.Core3.Controllers
{
    /// <summary>
    /// 登录---常规登录靠的是Cookie/Session
    /// </summary>
    public class FourthController : Controller
    {
        #region Identity
        private readonly ILogger<FourthController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestServiceA _iTestServiceA;
        private readonly ITestServiceB _iTestServiceB;
        private readonly ITestServiceC _iTestServiceC;
        private readonly ITestServiceD _iTestServiceD;
        private readonly IServiceProvider _iServiceProvider;
        private readonly IConfiguration _iConfiguration;

        //private readonly DbContext _dbContext;
        private readonly IUserService _iUserService;

        public FourthController(ILogger<FourthController> logger,
            ILoggerFactory loggerFactory
            , ITestServiceA testServiceA
            , ITestServiceB testServiceB
            , ITestServiceC testServiceC
            , ITestServiceD testServiceD
            , IServiceProvider serviceProvider
            , IConfiguration configuration
            , IUserService userService)
        //, DbContext dbContext)
        {
            this._logger = logger;
            this._loggerFactory = loggerFactory;
            this._iTestServiceA = testServiceA;
            this._iTestServiceB = testServiceB;
            this._iTestServiceC = testServiceC;
            this._iTestServiceD = testServiceD;
            this._iServiceProvider = serviceProvider;
            this._iConfiguration = configuration;
            //this._dbContext = dbContext;
            this._iUserService = userService;
        }
        #endregion

        /// <summary>
        /// 内部和注入的  有啥差别  证明
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //using (JDDbContext context = new JDDbContext())
            //{
            //    var user = context.Set<User>().First(u => u.Id > 1);
            //    base.ViewBag.UserName = user.Name;
            //}

            //var user = this._dbContext.Set<User>().First(u => u.Id > 1);
            //base.ViewBag.UserName = user.Name;

            //var userList = this._dbContext.Set<User>().Where(u => u.Id > 1).OrderBy(u => u.Id).Skip(1).Take(5);
            //base.ViewBag.UserList = userList;

            //using (JDDbContext context = new JDDbContext())
            //{
            //    var userList1 = context.Set<User>().Where(u => u.Id > 1).OrderBy(u => u.Id).Skip(1).Take(5);
            //    base.ViewBag.UserList1 = userList1;
            //}//对象是构造函数注入  和方法内获取的生命周期是不一样的

            var userList = this._iUserService.QueryPage<User, int>(u => u.Id > 1, 5, 1, u => u.Id);
            base.ViewBag.UserList = userList.DataList;

            return View();
        }

        [HttpGet]//响应get请求
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        //[CustomAllowAnonymous]
        public ActionResult Login(string name, string password, string verify)
        {
            string verifyCode = base.HttpContext.Session.GetString("CheckCode");
            if (verifyCode != null && verifyCode.Equals(verify, StringComparison.CurrentCultureIgnoreCase))
            {
                if ("Eleven".Equals(name) && "123456".Equals(password))
                {
                    CurrentUser currentUser = new CurrentUser()
                    {
                        Id = 123,
                        Name = "Eleven",
                        Account = "Administrator",
                        Email = "57265177",
                        Password = "123456",
                        LoginTime = DateTime.Now
                    };
                    #region Cookie/Session 自己写
                    //base.HttpContext.SetCookies("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser), 30);
                    //base.HttpContext.Session.SetString("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser));
                    #endregion
                    //过期时间全局设置

                    #region MyRegion
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,name),
                        new Claim("password",password),//可以写入任意数据
                        new Claim("Account","Administrator")
                    };
                    var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                    }).Wait();//没用await
                    //cookie策略--用户信息---过期时间
                    #endregion

                    return base.Redirect("/Home/Index");
                }
                else
                {
                    base.ViewBag.Msg = "账号密码错误";
                }
            }
            else
            {
                base.ViewBag.Msg = "验证码错误";
            }
            return View();
        }


        public ActionResult VerifyCode()
        {
            string code = "";
            Bitmap bitmap = VerifyCodeHelper.CreateVerifyCode(out code);
            base.HttpContext.Session.SetString("CheckCode", code);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }

        [HttpPost]
        //[CustomAllowAnonymous]
        public ActionResult Logout()
        {
            #region Cookie
            base.HttpContext.Response.Cookies.Delete("CurrentUser");
            #endregion Cookie

            #region Session
            CurrentUser sessionUser = base.HttpContext.GetCurrentUserBySession();
            if (sessionUser != null)
            {
                this._logger.LogDebug(string.Format("用户id={0} Name={1}退出系统", sessionUser.Id, sessionUser.Name));
            }
            base.HttpContext.Session.Remove("CurrentUser");
            base.HttpContext.Session.Clear();
            #endregion Session

            #region MyRegion
            //HttpContext.User.Claims//其他信息
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            #endregion
            return RedirectToAction("Index", "Home"); ;
        }

    }
}