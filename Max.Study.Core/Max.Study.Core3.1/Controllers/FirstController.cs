using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core3.Controllers
{
    /// <summary>
    /// 换个马甲，做MVC
    /// </summary>
    public class FirstController : Controller
    {
        private readonly ILogger<FirstController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public FirstController(ILogger<FirstController> logger,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            this._loggerFactory = loggerFactory;
        }

        public IActionResult Index()
        {
            this._logger.LogWarning("This is FirstController-Index");
            this._loggerFactory.CreateLogger<FirstController>().LogWarning("This is FirstController-Index 1");

            base.ViewBag.User1 = "天天";
            base.ViewData["User2"] = "ganet";
            base.TempData["User3"] = "jerry";

            //session还需要配置？ 以前直接用就好了 ashx默认没有session
            //配置了2下 就可以用了
            string result = base.HttpContext.Session.GetString("User4");
            //声明接口时，只提供最基本的诉求，扩展性的都没有了，
            if (string.IsNullOrWhiteSpace(result))
            {
                base.HttpContext.Session.SetString("User4", "情深");
            }

            object name = "ivy";

            return View(name);
        }
    }
}