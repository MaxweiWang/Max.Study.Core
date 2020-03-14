using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Utility.Filters
{
    /// <summary>
    /// 这是一个Action的Filter`  但是用作权限验证
    /// </summary>
    public class CustomAuthorizeActionFilterAttribute : Attribute, IActionFilter
    {
        private ILogger<CustomAuthorizeActionFilterAttribute> _logger = null;
        public CustomAuthorizeActionFilterAttribute(ILogger<CustomAuthorizeActionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //取出Session
            var strUser = context.HttpContext.Session.GetString("CurrentUser");
            if (!string.IsNullOrWhiteSpace(strUser))
            {
                CurrentUser currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrentUser>(strUser);
                _logger.LogDebug($"userName is {currentUser.Name}");
            }
            else
            { 
                context.Result = new RedirectResult("~/Fourth/Login");
            }
             
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //context.HttpContext.Response.WriteAsync("ActionFilter Executed!");
            Console.WriteLine("ActionFilter Executed!");
            //this._logger.LogDebug("ActionFilter Executed!");
        }

    }
}
