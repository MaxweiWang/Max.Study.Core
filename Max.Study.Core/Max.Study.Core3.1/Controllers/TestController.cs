using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        #region Identity
        private IConfiguration _IConfiguration = null;
        private ILogger<TestController> _logger = null;
        public TestController(IConfiguration configuration, ILogger<TestController> logger)
        {
            this._IConfiguration = configuration;
            this._logger = logger;
        }
        #endregion
        [HttpGet]
        public IActionResult Get()
        {
            this._logger.LogWarning("TestController-Get 执行");
            string msg = null;

            using (ConsulClient consulClient = new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500")))//实例化当前的consul
            {
                //consulClient.Agent.Services()获取consul中注册的所有的服务
                Dictionary<string, AgentService> services = consulClient.Agent.Services().Result.Response;
                foreach (KeyValuePair<string, AgentService> kv in services)
                {
                    this._logger.LogWarning($"key={kv.Key},{kv.Value.Address},{kv.Value.ID},{kv.Value.Service},{kv.Value.Port} {string.Join(",", kv.Value.Tags)}");
                }

                //获取所有服务名字是"apiserviceTest"所有的服务
                var agentServices = services.Where(s => s.Value.Service.Equals("apiserviceTest", StringComparison.CurrentCultureIgnoreCase))
                   .Select(s => s.Value);

                //根据当前TickCount对服务器个数取模，“随机”取一个机器出来，避免“轮询”的负载均衡策略需要计数加锁问题
                //1 随机获取---均衡调度
                //var agentService = agentServices.ElementAt(Environment.TickCount % agentServices.Count());

                //2 权重--某台服务器厉害点，某台差一点
                #region MyRegion
                var serviceWeight = agentServices.Select(s =>
                 {
                     int weight = 1;//不设置就是1
                     if (s.Tags != null && s.Tags.Length > 0 && int.TryParse(s.Tags[0], out weight))
                     {
                     }
                     KeyValuePair<AgentService, int> keyValuePair = new KeyValuePair<AgentService, int>(s, weight);
                     return keyValuePair;//3   5
                 });
                List<AgentService> serviceList = new List<AgentService>();//总数等于权重的和
                foreach (var sw in serviceWeight)
                {
                    for (int i = 0; i < sw.Value; i++)
                    {
                        serviceList.Add(sw.Key);
                    }
                }
                int total = serviceWeight.Sum(s => s.Value);
                Random random = new Random();
                int index = random.Next(0, total);//左边闭区间  右边开区间
                var agentService = serviceList[index];
                #endregion

                //base.HttpContext.Request.Headers  客户端就可以完成控制
                //可以根据tag--根据用户ip--等各种因素来做调度
                msg = $"{agentService.Address},{agentService.ID},{agentService.Service},{agentService.Port}";
                this._logger.LogWarning(msg);
            }
            return new JsonResult(new
            {
                Id = 123,
                Name = "Eleven",
                Remark = msg
            });
        }



        [HttpGet]
        [Route("api/Test/Invoke")]
        public IActionResult Invoke()
        {
            this._logger.LogWarning("TestController-Invoke 执行");
            string urlDefault = $"http://apiserviceTest/api/values";//我准备调用一下日志服务

            //http://apiserviceTest/api/values 转换为 http://127.0.0.1:5726/api/values
            string urlTarget = this.ResolveUrlAsync(urlDefault);

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage requestMsg = new HttpRequestMessage();
                //httpClient.DefaultRequestHeaders.Add("key", "value");
                requestMsg.Method = HttpMethod.Get;
                requestMsg.RequestUri = new Uri(urlTarget);

                var result = httpClient.SendAsync(requestMsg).Result;
                return new JsonResult(new
                {
                    Id = 123,
                    Name = "Eleven",
                    result.StatusCode,
                    urlTarget,
                    Content = result.Content.ReadAsStringAsync().Result,
                });
            }
        }

        private string ResolveUrlAsync(string url)
        {
            Uri uri = new Uri(url);
            string serviceName = uri.Host;//apiserviceTest
            string rootUrl = this.GetServiceAddress(serviceName);
            return uri.Scheme + "://" + rootUrl + uri.PathAndQuery;
        }

        private string GetServiceAddress(string serviceName)
        {
            using (ConsulClient consulClient = new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500")))
            {
                Dictionary<string, AgentService> services = consulClient.Agent.Services().Result.Response;
                var agentServices = services.Where(s => s.Value.Service.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase))
                   .Select(s => s.Value);
                var agentService = agentServices.ElementAt(Environment.TickCount % agentServices.Count());
                return $"{agentService.Address}:{agentService.Port}";
            }
        }
    }
}