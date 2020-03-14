using Microsoft.Extensions.DependencyInjection;
using Max.Core.Interface;
using Max.Core.Service;
using System;
//using System.Web.Services.Description;
using ServiceCollection = Microsoft.Extensions.DependencyInjection.ServiceCollection;

namespace Max.Core.ConsoleTest
{

    /// <summary>
    /// 1 深度解析.NetFrameWork/CLR/C# ，C#6/C#7新语法，理解.NetCore
    /// 2 Asp.Net Core2.2解读，MVC6应用、Session组件支持
    ///    
    /// 1 Core基于IIS的部署。
    /// 2 Core配置文件读取，Log4Net整合
    /// 3 自带依赖注入ServiceCollection解析
    /// 4 定制第三方依赖注入容器Autofac及AOP扩展
    /// 
    /// 欢迎大家来到.Net 高级班的Vip课程，我是Richard 老师；
    /// 、
    /// 
    /// 
    ///部署：
    ///      确保Core运行时正常安装
    ///      选择发布项目，发布都指定文件夹；
    ///      新建站点
    ///      程序池里设置无托管代码
    ///      
    /// 
    /// appsettings：对应Webconfig，存储格式也变了，Json；
    /// asp: confguartionManger
    /// Core：依赖于Configuration
    /// 通过Xpath语法来读取配置文件的数据
    ///  
    /// 
    /// IServiceCollection : 其实是一个容器 
    ///   容器的使用：
    ///            实例化一个容器；
    ///            注册
    ///            获取服务
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            ///容器的使用：
            ///            实例化一个容器；
            ///            注册
            ///            获取服务
            IServiceCollection container = new ServiceCollection();
            // IServiceCollection
            container.AddTransient<ITestServiceA, TestServiceA>();  // 瞬时生命周期  每一次获取的对象都是新的对象
            container.AddSingleton<ITestServiceB, TestServiceB>(); // 单例生命周期  在容器中永远只有当前这一个
            container.AddScoped<ITestServiceC, TestServiceC>();    //当前请求作用域内  只有当前这个实例

            container.AddSingleton<ITestServiceD>(new TestServiceD());  // 也是单例生命周期

            ServiceProvider provider = container.BuildServiceProvider();

            ITestServiceA testA = provider.GetService<ITestServiceA>();
            ITestServiceA testA1 = provider.GetService<ITestServiceA>();
            Console.WriteLine(object.ReferenceEquals(testA, testA1));

            ITestServiceB testB = provider.GetService<ITestServiceB>();
            ITestServiceB testB1 = provider.GetService<ITestServiceB>();
            Console.WriteLine(object.ReferenceEquals(testB, testB1));
             
            ITestServiceC testC = provider.GetService<ITestServiceC>();
            ITestServiceC testC1 = provider.GetService<ITestServiceC>();
            Console.WriteLine(object.ReferenceEquals(testC, testC1));
             
            IServiceScope scope = provider.CreateScope();
            ITestServiceC testc3 = provider.GetService<ITestServiceC>();
            var testc4 = scope.ServiceProvider.GetService<ITestServiceC>();
            Console.WriteLine(object.ReferenceEquals(testc3, testc4));
             
            ITestServiceD testD = provider.GetService<ITestServiceD>();
            ITestServiceD testD1 = provider.GetService<ITestServiceD>();
            Console.WriteLine(object.ReferenceEquals(testD, testD1));
             
            //Console.WriteLine("Hello World!");

            //var user = new
            //{
            //    Id = 11,
            //    Name = "Richard"
            //};
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user));


            //Console.WriteLine("**************************************");
            //{
            //    SharpSix six = new SharpSix();
            //    People people = new People()
            //    {
            //        Id = 505,
            //        Name = "马尔凯蒂"
            //    };
            //    six.Show(people);
            //}

            //Console.WriteLine("**************************************");
            //{
            //    SharpSeven seven = new SharpSeven();
            //    seven.Show();
            //}


        }
    }
}
