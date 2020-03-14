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

namespace Max.Study.Core2.Controllers
{

    /// <summary>
    /// log4Net 在控制器中写日志 
    /// core 内置了容器
    /// </summary>
    [TypeFilter(typeof(CustomControllerActionFilterAttribute),Order =-1)]
    public class ThirdController : Controller
    {
        private ILoggerFactory _factory = null;
        private ILogger<SecondController> _ilogger = null;

        private ITestServiceA _testServiceA = null;
        private ITestServiceB _testServiceB = null;
        private ITestServiceC _testServiceC = null;
        private ITestServiceD _testServiceD = null;
        private IA _a = null;

        public ThirdController(ILoggerFactory factory, ILogger<SecondController> ilogger,
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


        //[CustomActionFilterAttribute()]
        //[TypeFilter(typeof(CustomActionFilterAttribute),Order =0)]
        [ServiceFilter(typeof(CustomActionFilterAttribute),Order =-2)]
        public IActionResult Index()
        {
            //_a.Show(); 
            //_testServiceA.Show(); 
            //var ifactorylogger = _factory.CreateLogger<SecondController>();
            //ifactorylogger.LogDebug("this is ILoggerFactory logger");

            //_ilogger.LogDebug("this is ILogger<SecondController> debugger ");
            return View();
        }


    }
}