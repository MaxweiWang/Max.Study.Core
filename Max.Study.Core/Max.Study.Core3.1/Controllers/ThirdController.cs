using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Max.Core.Interface;
using Max.Study.Core3.Utility;
using Microsoft.AspNetCore.Authorization;

namespace Max.Study.Core3.Controllers
{
    /// <summary>
    /// 今天的代码量特别大，所以会有复制粘贴改
    /// Asp.NetCore--AOP--Filter---
    /// 面向切面编程--做一些面向对象做不到的事儿
    /// 
    /// AOP的事儿：
    /// Filter--管道模型，可以随意扩展随意high，进不去Action里面，也进不去View里面
    /// 想更进一步的AOP，就需要使用autofac的AOP扩展
    /// 
    /// </summary>
    //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]//控制器生效
    [CustomControllerFilterAttribute]
    //[TypeFilter(typeof(CustomActionCheckFilterAttribute))]
    [Authorize]
    public class ThirdController : Controller
    {
        #region Identity
        private readonly ILogger<ThirdController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestServiceA _iTestServiceA;
        private readonly ITestServiceB _iTestServiceB;
        private readonly ITestServiceC _iTestServiceC;
        private readonly ITestServiceD _iTestServiceD;
        private readonly IServiceProvider _iServiceProvider;
        private readonly IConfiguration _iConfiguration;
        public ThirdController(ILogger<ThirdController> logger,
            ILoggerFactory loggerFactory
            , ITestServiceA testServiceA
            , ITestServiceB testServiceB
            , ITestServiceC testServiceC
            , ITestServiceD testServiceD
            , IServiceProvider serviceProvider
            , IConfiguration configuration)
        {
            this._logger = logger;
            this._loggerFactory = loggerFactory;
            this._iTestServiceA = testServiceA;
            this._iTestServiceB = testServiceB;
            this._iTestServiceC = testServiceC;
            this._iTestServiceD = testServiceD;
            this._iServiceProvider = serviceProvider;
            this._iConfiguration = configuration;
        }
        #endregion

        /// <summary>
        /// 不是Action增加try catch 而是Filter
        /// 特性是编译时确定， this._logger是运行时才生成的，特性的构造函数只能是常量，不能是变量
        /// </summary>
        /// <returns></returns>
        //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]
        //[TypeFilter(typeof(CustomExceptionFilterAttribute))]
        //[CustomIOCFilterFactoryAttribute(typeof(CustomExceptionFilterAttribute))]//
        public IActionResult Index()
        {
            this._logger.LogWarning("This is Third Index");
            string AllowedHosts = this._iConfiguration["AllowedHosts"];
            string writeConn = this._iConfiguration["ConnectionStrings:Write"];
            string readConn = this._iConfiguration["ConnectionStrings:Read:0"];
            string[] _SqlConnectionStringRead = this._iConfiguration.GetSection("ConnectionStrings").GetSection("Read").GetChildren().Select(s => s.Value).ToArray();

            string allowedHost = this._iConfiguration["AllowedHost"].ToString();//异常

            return View();
        }

        /// <summary>
        /// 全局---控制器---Action  Order默认0，从小到大执行  可以负数
        /// </summary>
        /// <returns></returns>
        [CustomActionFilterAttribute(Order = 10)]
        [CustomActionFilterAttribute]
        //[CustomActionCacheFilterAttribute(Order = -1)]
        //[IResourceFilter]
        [CustomResourceFilterAttribute]
        public IActionResult Info()
        {
            Console.WriteLine($"This is {nameof(ThirdController)} Info");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        //那什么时候用中间件  什么时候用Filter---因为Filter是MVC的，中间件能知道action controller--中间件是全部请求都要通过的，Filter可以针对方法/controller---粒度不同的，合适选择

    }
}