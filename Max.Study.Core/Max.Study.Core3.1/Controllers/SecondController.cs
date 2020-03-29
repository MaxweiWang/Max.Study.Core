using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Interface;
//using Zhaoxi.AspNetCore3_1.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Max.Study.Core3.Controllers
{
    public class SecondController : Controller
    {
        private readonly ILogger<SecondController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestServiceA _iTestServiceA;
        private readonly ITestServiceB _iTestServiceB;
        private readonly ITestServiceC _iTestServiceC;
        private readonly ITestServiceD _iTestServiceD;
        private readonly IServiceProvider _iServiceProvider;
        /// <summary>
        /// 来自DI依赖注入---构造函数注入---构造A对象依赖B对象，自动初始化B对象传入
        /// 
        /// 有个容器，负责构造SecondController(反射)--反射构造函数--需要B--再构造B---然后再构造A
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loggerFactory"></param>
        public SecondController(ILogger<SecondController> logger,
            ILoggerFactory loggerFactory
            , ITestServiceA testServiceA
            , ITestServiceB testServiceB
            , ITestServiceC testServiceC
            , ITestServiceD testServiceD
            , IServiceProvider serviceProvider)
        //一次注入这么多，可能用不上，能不能我自己生成? 不建议，
        //1 大部分对象生成其实很快，且不占资源--特殊可以自己控制
        //2 对象生命周期不同，注入的是跟controller的，action是跟方法的
        {
            this._logger = logger;
            this._loggerFactory = loggerFactory;
            this._iTestServiceA = testServiceA;
            this._iTestServiceB = testServiceB;
            this._iTestServiceC = testServiceC;
            this._iTestServiceD = testServiceD;
            this._iServiceProvider = serviceProvider;
        }
        /// <summary>
        /// 纯属测试 毫无意义
        /// </summary>
        private static ITestServiceC _iTestServiceCStatic = null;
        private static ITestServiceB _iTestServiceBStatic = null;

        public IActionResult Index()
        {
            //ITestServiceA testServiceA = new TestServiceA();
            this._logger.LogWarning("This is SecondController Index");

            var c = this._iServiceProvider.GetService<ITestServiceC>();
            Console.WriteLine($"cc {object.ReferenceEquals(this._iTestServiceC, c)}");//T/F

            if (_iTestServiceCStatic == null)
            {
                _iTestServiceCStatic = _iTestServiceC;
            }
            else
            {
                Console.WriteLine($"C&C {object.ReferenceEquals(this._iTestServiceC, _iTestServiceCStatic)}");//两次不同的请求  //T/F
            }

            if (_iTestServiceBStatic == null)
            {
                _iTestServiceBStatic = _iTestServiceB;
            }
            else
            {
                Console.WriteLine($"B&B：{object.ReferenceEquals(this._iTestServiceB, _iTestServiceBStatic)}");//两次不同的请求  //T/F
            }


            this._iTestServiceA.Show();
            this._iTestServiceB.Show();
            this._iTestServiceC.Show();
            this._iTestServiceD.Show();
            return View();
        }
    }
}