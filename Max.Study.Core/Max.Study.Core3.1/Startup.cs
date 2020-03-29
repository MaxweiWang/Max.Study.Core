using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Max.Study.Core3.Utility;
using Max.Study.Core3.Utility.ConsulExtend;
using Microsoft.AspNetCore.Authentication.Cookies;
using Max.Core.Interface;
using Max.Core.Service;
using Max.EFCore3_1.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Max.Study.Core3
{
    public class Startup
    {
        /// <summary>
        /// 自有妙用
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           // StaticConstraint.Init(s => configuration[s]);
        }

        public IConfiguration Configuration { get; }


        /// <summary>
        /// 初始化，最早执行且只执行一次的
        /// 给IOC容器增加映射关系，
        /// IServiceCollection--容器--就能完成DI--
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            //services.AddControllersWithViews();
            //services.AddRazorPages();//约等于AddMvc() 就是3.0把内容拆分的更细一些，能更小的依赖
            services.AddControllersWithViews(
                 options =>
                 {
                     options.Filters.Add<CustomExceptionFilterAttribute>();//全局注册
                    options.Filters.Add<CustomGlobalFilterAttribute>();
                 }).AddRazorRuntimeCompilation();//修改cshtml后能自动编译
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Fourth/Login");
                    options.AccessDeniedPath = new PathString("/Home/Privacy");

                });//用cookie的方式验证，顺便初始化登录地址

            //services.AddScoped<DbContext, JDDbContext>();//下面这套还不是把我封装了一下

            services.AddScoped<IUserService, UserService>();


            //这个注入没有成功--注入是没问题的，构造函数也只是支持参数就好，错在注入的地方不能写DbContext
            services.AddDbContext<JDDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("JDDbConnection"));
            });


            //services.AddEntityFrameworkSqlServer()
            //      .AddDbContext<JDDbContext>(options =>
            //      {
            //          options.UseSqlServer(Configuration.GetConnectionString("JDDbConnection") //读取配置文件中的链接字符串
            //              );  //配置分页 使用旧方式
            //      });



            //services.AddScoped(typeof(CustomExceptionFilterAttribute));//不是直接new  而是容器生成 就可以自动注入了

            //只能构造函数注入--需要一个构造函数超集
            //services.AddTransient<ITestServiceA, TestServiceA>();//瞬时
            //services.AddSingleton<ITestServiceB, TestServiceB>();//单例
            //services.AddScoped<ITestServiceC, TestServiceC>();//作用域单例--一次请求一个实例
            ////作用域其实依赖于ServiceProvider（这个自身是根据请求的），跟多线程没关系
            //services.AddTransient<ITestServiceD, TestServiceD>();
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CustomAutofacModule>();
        }

        /// <summary>
        /// Http请求管道模型---就是Http请求被处理的步骤---Http请求是一段文本，被Kestrel解析得到HttpContext---然后被后台代码处理Request---返回Response---经由Kestrel回发到客户端
        /// 所谓管道，就是拿着HttpContext，经过多个步骤的加工，生成Response,就是管道
        /// Configure就是指定我们的代码如何去处理请求
        /// 
        /// 如果啥也不配置，但是默认是还有响应，是404
        /// 这个方法，叫请求级(所有请求生效)----页面级Home/Index
        /// 这个方法执行且只执行一次，是初始化
        /// 
        /// RequestDelegate是一个委托,接受一个HttpContext，执行操作，然后没有然后了
        /// IApplicationBuilder在Build之后 就是一个RequestDelegate
        /// Application--所谓管道，其实就应该是个RequestDelegate
        /// 
        /// 就像有一把手术刀，可以在流程的任意节点去扩展
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //app.Run(context => context.Response.WriteAsync("This is Hello World!"));
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync($"Url is {context.Request.Path.Value}");
            //});

            #region Use中间件
            //app.Run(c => c.Response.WriteAsync("Hello World!"));
            //////任何请求来了，只是返回个hello world    终结式
            //////所谓Run终结式注册，其实只是一个扩展方法，最终还不是得调用Use方法，

            ////IApplicationBuilder 应用程序的组装者
            ////RequestDelegate:传递一个HttpContext，异步操作下，不返回；也就是一个处理动作
            //// Use(Func<RequestDelegate, RequestDelegate> middleware) 委托，传入一个RequestDelegate，返回一个RequestDelegate
            ////ApplicationBuilder里面有个容器IList<Func<RequestDelegate, RequestDelegate>> _components
            ////Use就只是去容器里面添加个元素
            ////最终会Build()一下， 如果没有任何注册，就直接404处理一切
            ///*
            // foreach (var component in _components.Reverse())//反转集合  每个委托拿出来
            //{
            //    app = component.Invoke(app);
            //    //委托3-- 404作为参数调用，返回 委托3的内置动作--作为参数去调用委托(成为了委托2的参数)--循环下去---最终得到委托1的内置动作---请求来了HttpContext---
            //}
            // */
            ////IApplicationBuilder build之后其实就是一个RequestDelegate，能对HttpContext加以处理
            ////默认情况下，管道是空的，就是404；可以根据你的诉求，任意的配置执行，一切全部由开发者自由定制，框架只是提供了一个组装方式

            ////Func<RequestDelegate, RequestDelegate> middleware = next =>
            ////{
            ////    return new RequestDelegate(async context =>
            ////                    {
            ////                        await context.Response.WriteAsync("<h3>This is Middleware1 start</h3>");
            ////                        await Task.CompletedTask;
            ////                        await next.Invoke(context);//RequestDelegate--需要context返回Task
            ////                        await context.Response.WriteAsync("<h3>This is Middleware1 end</h3>");
            ////                    });
            ////};
            ////app.Use(middleware);

            //app.Use(next =>
            //{
            //    System.Diagnostics.Debug.WriteLine("this is Middleware1");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware1 start</h3>");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("<h3>This is Middleware1 end</h3>");
            //    });
            //});

            //app.Use(next =>
            //{
            //    System.Diagnostics.Debug.WriteLine("this is Middleware2");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware2 start</h3>");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("<h3>This is Middleware2 end</h3>");
            //    });
            //});
            //app.Use(next =>
            //{
            //    System.Diagnostics.Debug.WriteLine("this is Middleware3");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware3 start</h3>");
            //        //await next.Invoke(context);//注释掉，表示不再往下走
            //        await context.Response.WriteAsync("<h3>This is Middleware3 end</h3>");
            //    });
            //});

            //////1 Run 终结式  只是执行，没有去调用Next  
            //////一般作为终结点
            ////app.Run(async (HttpContext context) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Run");
            ////});
            ////app.Run(async (HttpContext context) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Run Again");
            ////});

            //////2 Use表示注册动作  不是终结点  
            //////执行next，就可以执行下一个中间件   如果不执行，就等于Run
            ////app.Use(async (context, next) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Use1 <br/>");
            ////    await next();
            ////    await context.Response.WriteAsync("Hello World Use1 End <br/>");
            ////});
            ////app.Use(async (context, next) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Use2 Again <br/>");
            ////    await next();
            ////});
            //////UseWhen可以对HttpContext检测后，增加处理环节;原来的流程还是正常执行的
            ////app.UseWhen(context =>
            ////{
            ////    return context.Request.Query.ContainsKey("Name");
            ////},
            ////appBuilder =>
            ////{
            ////    appBuilder.Use(async (context, next) =>
            ////    {
            ////        await context.Response.WriteAsync("Hello World Use3 Again Again Again <br/>");
            ////        await next();
            ////    });
            ////});

            ////app.Use(async (context, next) =>//没有调用 next() 那就是终结点  跟Run一样
            ////{
            ////    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            ////    //await next();
            ////});

            //////Map：根据条件指定中间件  指向终结点，没有Next
            //////最好不要在中间件里面判断条件选择分支；而是一个中间件只做一件事儿，多件事儿就多个中间件
            ////app.Map("/Test", MapTest);
            ////app.Map("/Eleven", a => a.Run(async context =>
            ////{
            ////    await context.Response.WriteAsync($"This is Advanced Eleven Site");
            ////}));
            ////app.MapWhen(context =>
            ////{
            ////    return context.Request.Query.ContainsKey("Name");
            ////    //拒绝非chorme浏览器的请求  
            ////    //多语言
            ////    //把ajax统一处理
            ////}, MapTest);

            ////Middleware类
            //app.UseMiddleware<ElevenStopMiddleware>();
            //app.UseMiddleware<ElevenMiddleware>();
            //app.UseMiddleware<ElevenSecondMiddleware>();
            ////app.Use(async (context, next) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            ////});
            #endregion

            #region middleware
            //Use很灵活，但是很复杂
            //终结式-- app.Use(_ => handler);---意味着没有下个步骤
            //app.Run(c => c.Response.WriteAsync("Hello World!"));

            //另外个use  可以不是request delegate
            //app.Use(async (context, next) =>//没有调用 next() 那就是终结点  跟Run一样
            //{
            //    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            //    //await next();
            //});
            //UseWhen可以对HttpContext检测后，增加处理环节;原来的流程还是正常执行的
            //app.UseWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //},
            //appBuilder =>
            //{
            //    appBuilder.Use(async (context, next) =>
            //    {
            //        await context.Response.WriteAsync("Hello World Use3 Again Again Again <br/>");
            //        //await next();
            //    });
            //});

            ////根据条件指定中间件 指向终结点，没有Next
            //app.Map("/Test", MapTest);
            //app.Map("/Eleven", a => a.Run(async context =>
            //{
            //    await context.Response.WriteAsync($"This is Advanced Eleven Site");
            //}));
            //app.MapWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //拒绝非chorme浏览器的请求  
            //    //多语言
            //    //把ajax统一处理
            //}, MapTest);
            //以上均为Use的封装，其实是为了熟悉的人方便，或者增加面试的复杂度

            ////UseMiddlerware 类--反射找
            //app.UseMiddleware<FirstMiddleWare>();
            //app.UseMiddleware<SecondMiddleWare>();
            //app.UseMiddleware<ThreeMiddleWare>();
            #endregion



            #region 环境参数
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #endregion

            #region 这些叫中间件，最终把请求交给MVC
            loggerFactory.AddLog4Net();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });
            app.UseRouting();
            app.UseAuthentication();//鉴权，检测有没有登录，登录的是谁，赋值给User
            app.UseAuthorization();//就是授权，检测权限
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            #endregion



            #region consul
            this.Configuration.RegistConsul();
            #endregion
        }
    }
}
