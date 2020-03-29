using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Max.Study.Core3
{
    /// <summary>
    /// 1 Asp.NetCore3.0Preview7环境配置，项目迁移
    /// 2 Razor动态编译，TempData序列化，添加区域
    /// 3 中间件源码解读，理解新管道处理模型
    /// 4 autofac新模式
    /// 5 EntityFrameworkCore3.0使用&封装&扩展日志
    /// 6 .NetCore3.0 WebApi开发应用，前后分离
    /// 
    /// 新环境配置：Asp.NetCore3.0 Preview7
    /// 暂时不适合直接上线项目，所以课程主要学习是2.2
    /// 1 只能是VS2019---VS2019下载地址和激活码
    /// 2 .NetCore3.0是需要独立安装运行时环境CLR
    ///   dotnet-hosting-3.0.0-preview7.19365.7-win.exe
    ///   这里还包含IIS需要的Module
    /// 3  SDK软件开发工具包--VS才有对应的模板
    ///   dotnet-sdk-3.0.100-preview7-012821-win-x64-.exe
    ///   工具--选项--环境---预览功能--Preview
    /// 
    /// .NetCore为什么可以跨平台？
    /// 因为微软出了一套可以在Linux运行的CLR
    /// .NetFramework里面CLR更新慢一些，Core的CLR变化很快
    /// 
    /// Core2.2是正式版，现在讲的3.0是预览版，会有蛮多的改动
    /// 计划帮大家完成从2.2到3.0的升级，解决变化，深入一下原理，
    /// 包括EFCore  Core的WebApi  搭建一套3.0的框架
    /// 此外，还有一个福利，
    /// 准备邀请学员基于3.0的框架完成一个真实项目，老师出钱(几千)
    /// 能真实基于Core做个项目，然后我来上线
    /// 
    /// Razor cshtml应该是在访问是会动态编译
    /// 3.0默认是没有动态编译---nuget添加AddRazorRuntimeCompilation--startup configservice添加下--
    /// 
    /// tempdata序列化时，只能是基础类型+int集合+字典，不能是自定义类型
    /// 
    /// 提供AspNetCore-master源码.zip是可以直接查看源代码
    /// 也可以通过https://github.com/aspnet/AspNetCore去查看
    /// 
    /// 区域添加一下：
    /// MVC区分区域是通过命名空间,这里不行
    /// 需要在控制器上面添加[Area("System")] [Route("System/[controller]/[action]")]   每个控制器都需要，所以可以继承个BaseAreaController
    /// 
    /// 把Core2.2的类库 给迁移到Core3.0  修改targetframework，更新引用
    /// 
    /// 管道处理模型--中间件
    /// Startup--Config里面去指定了Http请求管道
    /// 何谓http请求的管道呢？
    /// 就是对Http请求的一连串的处理过程
    /// 就是给你一个HttpContext，然后一步步的处理，最终的得到结果
    /// 
    /// Asp.Net请求管道： 请求最终会由一个具体的HttpHandler处理(page/ashx/mvchttphandler--action)
    ///                   但是还有多个步骤，被封装成事件--可以注册可以扩展--IHttpModule--提供了非常优秀的扩展性
    /// 有一个缺陷：太多管闲事儿了--一个http请求最核心是IHttpHandler--cookie Session  Cache NeginRequest endrequest maprequesthandler 授权----这些不一定非得有---但是写死了---默认认为那些步骤是必须的---跟框架的设计思想有关---.Net入门简单精通难---因为框架大包大揽，全家桶式，随便拖一下控件，写点数据库，一个项目就出来了---所以精通也难----也要付出代价，就是包袱比较重，不能轻装前行---.NetCore是一套全新的平台，已经不再向前兼容---设计更追求组件化，追求高性能---没有全家桶        
    /// 
    /// Asp.NetCore全新的请求管道：
    /// 默认情况，管道只有一个404
    /// 然后你可以增加请求的处理(UseEndPoint)---这就是以前handler，只包含业务处理
    /// 其他的就是中间件middleware
    /// 
    /// 
    /// 1 autofac新模式
    /// 2 EntityFrameworkCore3.0使用&封装&扩展日志
    /// 3 .NetCore3.0 WebApi开发应用，前后分离
    /// 
    /// 替换容器时，升级了
    /// a nuget--可以参考依赖项里面的autofac相关
    /// b UseServiceProviderFactory
    /// c ConfigureContainer(ContainerBuilder containerBuilder)
    /// 
    /// EntityFrameworkCore3.0
    /// 没有edmx，一般是code first  也没有自动创建
    /// 
    /// 先nuget一下efcore+efcoresqlserver
    /// a 从Framework生成实体+context，然后复制粘贴
    /// b JDDbContext构造函数不指定链接
    /// c protected override void OnConfiguring 添加链接
    /// d protected override void OnModelCreating改了个参数类型
    /// 
    /// 关于链接问题，肯定放在配置文件，配置文件怎么读取
    /// 1 内部写死度appsetting--不好 路径可能变化
    /// 2 dbcontext注入IConfiguration---没问题
    /// 3 关于配置文件，如果要用配置项，我们是不是都得注入IConfiguration，
    ///   但是一些静态方法需要使用配置文件
    ///   以前.NetFramework会把配置文件集中管理，做成静态，使用的时候直接拿，
    ///   还是一个StaticConstraint，然后在startup环境传递委托完成初始化
    ///   
    /// .NetCore不再出现静态，需要的话就通过单例模式
    /// 就可以从上到下保持全程依赖注入(改造不小)
    /// 
    /// WebApi:
    /// 直接添加了WebApi控制器,
    /// 这里不需要添加额外的路由，直接靠特性路由
    /// [Route("api/[controller]"), ApiController]  都不能少
    /// 此外，就是具体的action可能需要具体的特性路由
    /// 
    /// btnGet5的时候失败，大家自行研究！   
    /// 谁先搞定的（私聊给我为准，不允许群里发答案），前3名，8.88
    /// 
    /// WebApi的管道和MVC合并了，是一个了
    /// 权限Filter 异常的Filter 是通用的
    /// 
    /// .NetCore其实不难，有问题解决问题，可以搞定的
    /// 差实践！
    /// 
    /// 
    ///
    /// *********************微服务架构****************************
    /// 1 微服务架构解析，优缺点、挑战与转变
    /// 2 服务实例准备，Consul安装
    /// 3 Consul注册，心跳检测，服务发现
    /// 
    /// 微服务架构专题，基于Core
    /// 
    /// 命令行参数--AddCommandLine---启动时可以传递参数---然后可以Configuration["name"] 获取一下
    /// 
    /// 
    /// 服务注册与发现
    /// a  添加Webapi服务---添加log4net---注入到控制器--记录日志
    /// b  命令启动webapi服务---2个实例--不同端口
    /// c  准备consul--启动--nuget-consul
    /// d  网站启动后需要注册到consul
    /// e  添加health-check，健康检查
    /// f  在startup去注册下---然后启动多个实例
    /// g  去http://localhost:8500查看多个服务被发现和心跳检测 
    ///    
    /// 
    /// 1 Consul消费者，完成负载均衡
    /// 2 基于Ocelot搭建Gateway，完成服务调用
    /// 3 Ocelot+Consul整合使用，网关+服务注册发现
    /// 4 Ocelot配置解析，熔断-限流等功能实现
    ///
    /// 下面Eleven老师准备给大家写消费者，基于Consul来完成服务调用
    /// a 不准备再建个项目做消费者，直接就写在一个项目
    /// 
    /// 
    /// Ocelot:
    /// Core-Webapi项目--引入Ocelot---一组中间件---program替换配置文件--增加配置文件--startup去配置--全部换成ocelot---配置文件完成配置--即可完成网关访问
    /// 
    /// 
    /// 1 Ocelot配置进阶，缓存-限流-熔断-聚合等功能实现
    /// 2 解读IdentityServer4验证中心，OAuth-OpenID
    /// 3 分布式事务解析，CAP&Base，多模式解析
    /// 4 Docker&K8S快速发布部署
    /// 
    /// 完成了Ocelot+Consul整合
    /// a  恢复环境--Consul+2个服务实例--测试调用
    /// b  ocelot启动，结合配置文件，结合Consul 完成请求转发以及服务发现
    /// 
    /// 网关不仅是做请求的转发--中间层，我的地盘听我的，我给你show一下
    /// a  缓存---配置下就可以了
    /// b  限流---限制单位时间内请求的数量--反爬虫&秒杀
    /// c  熔断---家里保险丝过载就会熔断，全停，可以保护别人家正常的电流
    ///           A股，出现了超出平常的异动，直接停掉全部交易
    ///           达成某些条件后，接口将暂时不再提供服务
    ///    nuget--polly---AddPolly()
    /// d  请求合并---上端请求一下，网关会请求多个服务，结果合并，仅get
    /// 
    /// Session---Token---OAuth+OpenId----IdentityServer4---AuthorizationServer---获取Token
    /// 
    /// 明天纯理论--分布式事务-Docker-K8S
    /// 下一期我有个想法，计划把Framework和Core并行式学习
    /// 
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddCommandLine(args)//支持命令行
           .Build();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddFilter("System", LogLevel.Warning);
                    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
                    loggingBuilder.AddLog4Net();//文件路径
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())//设置工厂来替换实例
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
