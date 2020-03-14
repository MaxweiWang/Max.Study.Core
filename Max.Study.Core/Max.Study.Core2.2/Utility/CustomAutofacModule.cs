using Autofac;
using Autofac.Extras.DynamicProxy;
using Max.Core.Interface;
using Max.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Study.Core2.Utility
{
    public class CustomAutofacModule:Module
    { 
        /// <summary>
        ///  当前这Module 专用做服务注册
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(a=>new CustomAutofacAOP()); //autofac 允许使用Aop
             
            builder.RegisterType<TestServiceA>().As<ITestServiceA>().SingleInstance();
            builder.RegisterType<TestServiceB>().As<ITestServiceB>().SingleInstance();
            builder.RegisterType<TestServiceC>().As<ITestServiceC>().SingleInstance();
            builder.RegisterType<TestServiceD>().As<ITestServiceD>().SingleInstance();
            // 允许当前注册的这个服务实例使用Aop
            builder.RegisterType<A>().As<IA>().EnableInterfaceInterceptors();
        }
    }
}
