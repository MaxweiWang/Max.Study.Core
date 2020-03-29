using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        #region Identity
        private IConfiguration _IConfiguration = null;
        private ILogger<HealthController> _logger = null;
        public HealthController(IConfiguration configuration, ILogger<HealthController> logger)
        {
            this._IConfiguration = configuration;
            this._logger = logger;
        }
        #endregion

        [HttpGet]
        public IActionResult Check()
        {
            this._logger.LogWarning($"{this._IConfiguration["port"]}-Health Check!");
            return Ok();//200
        }

    }
}