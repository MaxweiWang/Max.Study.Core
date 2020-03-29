using Autofac;
using Max.Core.Interface;
using Max.Core.Service;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Max.Study.Core3.Utility
{
    public class CustomAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var builder = new ContainerBuilder();
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);
            builder.RegisterType<ApplicationPartManager>().AsSelf().SingleInstance();
            builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray()).PropertiesAutowired();
            //containerBuilder.RegisterType<FirstController>().PropertiesAutowired();

            //containerBuilder.Register(c => new CustomAutofacAop());//aop注册
            containerBuilder.RegisterType<TestServiceA>().As<ITestServiceA>().SingleInstance().PropertiesAutowired();
            containerBuilder.RegisterType<TestServiceC>().As<ITestServiceC>();
            containerBuilder.RegisterType<TestServiceB>().As<ITestServiceB>();
            containerBuilder.RegisterType<TestServiceD>().As<ITestServiceD>();
           // containerBuilder.RegisterType<TestServiceE>().As<ITestServiceE>();

            //containerBuilder.RegisterType<A>().As<IA>();//.EnableInterfaceInterceptors();

            //containerBuilder.Register<FirstController>();

            //containerBuilder.RegisterType<JDDbContext>().As<DbContext>();
            //containerBuilder.RegisterType<CategoryService>().As<ICategoryService>();

            //containerBuilder.RegisterType<UserServiceTest>().As<IUserServiceTest>();
        }

    }
}
    //public class CustomAutofacAop : IInterceptor
    //{
    //    public void Intercept(IInvocation invocation)
    //    {
    //        Console.WriteLine($"invocation.Methond={invocation.Method}");
    //        Console.WriteLine($"invocation.Arguments={string.Join(",", invocation.Arguments)}");

    //        invocation.Proceed(); //继续执行

    //        Console.WriteLine($"方法{invocation.Method}执行完成了");
    //    }
    //}

    //public interface IA
    //{
    //    void Show(int id, string name);
    //}

    //[Intercept(typeof(CustomAutofacAop))]
    //public class A : IA
    //{
    //    public void Show(int id, string name)
    //    {
    //        Console.WriteLine($"This is {id} _ {name}");
    //    }
    //}
