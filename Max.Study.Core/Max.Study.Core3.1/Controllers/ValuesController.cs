using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        #region Identity
        private IConfiguration _IConfiguration = null;
        private ILogger<ValuesController> _logger = null;
        public ValuesController(IConfiguration configuration, ILogger<ValuesController> logger)
        {
            this._IConfiguration = configuration;
            this._logger = logger;
        }
        #endregion

        [HttpGet]
        public IActionResult Get()
        {
            this._logger.LogWarning("ValuesController-Get 执行");
            return new JsonResult(new
            {
                Id = 123,
                Name = "cwer",
                IP = this._IConfiguration["ip"],
                port = this._IConfiguration["port"],
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                urls = this._IConfiguration["urls"],
                //Remark = this._IConfiguration["Consul:ServcieRemark"],
                //CurrentPort = this._IConfiguration["Port"],
                CurrentPath = base.HttpContext.Request.Path,

            });
        }

        [HttpGet]
        [Route("Timeout")]
        public IActionResult Timeout()
        {
            this._logger.LogWarning("ValuesController-Timeout 执行");
            Thread.Sleep(6000);
            return new JsonResult(new
            {
                Id = 123,
                Name = "XM",
                port = this._IConfiguration["port"],
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
            });
        }
        [HttpGet]
        [Route("Exeception")]
        public IActionResult Exeception()
        {
            this._logger.LogWarning("ValuesController-Exeception 执行");
            throw new NotImplementedException();
        }
    }
}