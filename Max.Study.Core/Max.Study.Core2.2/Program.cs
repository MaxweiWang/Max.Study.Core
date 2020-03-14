using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core2
{

    /// <summary>
    /// 1 深度解析.NetFrameWork/CLR/C# ，C#6/C#7新语法，理解.NetCore
    /// 2 Asp.Net Core2.2解读，MVC6应用、Session组件支持
    /// 3 Asp.NetCore发布，IIS部署，配置文件读取，Log4Net整合
    ///  asp.net--网站托管在IIS--IIS负责监听-转发请求--响应客户端
    ///  .net core--控制台--CreateWebHostBuilder(内置了服务器)--启动了服务器--负责监听-转发请求--响应客户端
    ///  KestrelServer 跨平台的服务器
    ///  (IIS只能做反向代理，不再做啥监听)
    ///  
    /// 
    /// Asp.Net      不负责请求的监听-转发-响应(IIS)，封装了管道处理模型，只写业务处理逻辑
    /// Asp.NetCore  是个控制台，请求监听-转发-响应都是自己完成的，包括管道模型也是自定义，
    ///              这里不再像以前那样，什么都封装好了，开发者什么也不知道
    ///              封装的少，东西就少
    ///              
    /// 
    /// Core部署在IIS下：
    ///         确保在Host已经安装的情况下，（在IIS功能视图下的模块中查看是否存在AspNetCoreMoudleV2）
    ///         发布 到文件夹下，
    ///         在IIS新建一个站点，站点的文件路径指向发布的文件夹下
    ///         程序池基本设置设置为无托管代码
    ///         
    /// Asp读取配置文件：依赖于ConfiguartionManager
    /// 
    /// log4net 集成到Core：
    ///     引入log4net  
    ///     Microsoft.Extensions.Logging.Log4Net.AspNetCore 
    ///     添加log4net 配置文件 （设置配置文件属性为始终复制，默认配置文件放在根目录下）
    ///     注入ILoggerFactory
    ///     创建Ilogger 对象
    ///     写日志 
    ///      
    ///  IServiceCollection : 其实是一个容器 
    ///  容器的使用：
    ///            实例化一个容器；
    ///            注册
    ///            获取服务
    //  整合Autofac
    /// 1、引入autofac Autofac.Extensions.DependencyInjection
    /// 2、ConfigureServices需要返返回值 IServiceProvider
    /// 3、实例化容器
    /// 4、注册服务
    /// 5、返回AutofacServiceProvider 的实例
    /// 
    /// Autofac支持Aop
    /// 
    /// 在Framework环境下：权限特性  Action/Result Exception
    /// 
    ///         因为特性是随编译之后存在
    /// 
    /// Core： 加了ResourceFilter Action/Result Exception
    ///         Action/Result Exception三个特性没有什么变化。
    ///         
    ///分别对全局、控制器、action 注册了 ActionFiler 执行顺序：  全局OnActionExecuting--控制器的OnActionExecuting--  Action OnActionExecuting Action--OnActionExecutingd--Action OnActionExecuted 控制器的 OnActionExecuted  全局的OnActionExecuted  类似于一个俄罗斯套娃 
    ///
    /// Order 是排序，执行顺序按照从小到大的顺序执行；
    /// 
    /// ResourceFilter  在控制器实例化之前执行，分别有 OnResourceExecuted OnResourceExecuting  适合做缓存；
    ///  
    /// 管道处理模型：  framework环境下：19 个事件，是一个全家桶，把所有需要的东西全部封装在内了，如果需要扩展，就注册不同的事件，进行扩展，但是执行顺序是固定不变得；
    /// 
    /// core:  把每一个执行块自由组装起来，自行调整顺序，最终形成一个链子一样。
    ///         MiddleWare （中间件）  执行顺序还是一个俄罗斯套娃
    ///         
    /// 
    ///  1 搬迁MVC5的权限认证到Asp.Net Core
    ///  2 Asp.Net Core认证&授权原理机制
    ///  3 用户登录权限验证，基于Cookie的认证&授权
    ///  4 Scheme、Role、Policy扩展
    /// .net Framework环境授权：
    ///     在登录的时候写入Session
    ///     在需要控制权限的方法上标记一个权限特性 实现在方法执行前对Session 进行判断
    ///     如果有Session  就有权限 
    ///     
    /// 以上做法是比较局限
    /// 
    /// 
    /// .Core 下的权限认证 
    ///     来自于AuthenticationHttpContextExtensions扩展
    ///     6大方法 可以自行扩展这个6个方法：需要自定一个hander，hander 需要继承实现IAuthenticationHandler, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    ///     分别实现6个方法，需要指定在Core中使用： services.AddAuthenticationCore(options => options.AddScheme<MyHandler>("myScheme", "demo myScheme"));
    ///     
    /// 如果使用了Sechme验证，验证不通过的时候，就默认跳转到Account/Login?ReturnUrl=%2FFourth%2FIndex
    /// 
    /// 权限验证来自于IAuthorizeData  : AuthenticationSchemes Policy Roles
    /// 权限验证支持Action 控制器  全局 三种注册方式
    /// 也支持匿名 和之前的一样。
    ///
    ///

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)  // 创建一个服务器  服务器是跨平台的
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((context, LoggingBuilder) =>
            {
                LoggingBuilder.AddFilter("System", LogLevel.Warning); // 忽略系统的其他日志
                LoggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                LoggingBuilder.AddLog4Net();
            })
            .UseStartup<Startup>();
    }
}
