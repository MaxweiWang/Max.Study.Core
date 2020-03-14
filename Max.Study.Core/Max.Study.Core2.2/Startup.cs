using System;
using System.Collections.Generic;
using System.Security.Claims;
using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Max.Core.Interface;
using Max.Study.Core2.Utility;
using Max.Core.Service;
using Max.Core.Utility.Authentications;
using Max.Core.Utility.Filters;
using Max.Core.Utility.Middleware;

namespace Max.Study.Core2
{
    /// <summary>
    /// Startup是一个固定类， 名字必须是当前这个名字
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构造函数的注入
        /// </summary>
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 整合Autofac
        /// 1、引入autofac Autofac.Extensions.DependencyInjection
        /// 2、ConfigureServices需要返返回值 IServiceProvider
        /// 3、实例化容器
        /// 4、注册服务
        /// 5、返回AutofacServiceProvider 的实例
        /// </summary>
        /// <param name="services"></param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region 设置自己的schema的handler 
            //services.AddAuthenticationCore(options => options.AddScheme<MyHandler>("myScheme", "demo myScheme"));
            #endregion

            #region 支持 policy 认证授权的服务  

            // 指定通过策略验证的策略列
            services.AddSingleton<IAuthorizationHandler, AdvancedRequirement>();

            services.AddAuthorization(options =>
            {
                //AdvancedRequirement可以理解为一个别名
                options.AddPolicy("AdvancedRequirement", policy =>
                {
                    policy.AddRequirements(new NameAuthorizationRequirement("1"));
                });
            }).AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Fourth/Login");
                options.ClaimsIssuer = "Cookie";
            });

            #endregion

            #region  Schame 验证

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;// "Richard";//  
            //})
            //.AddCookie(options =>
            //{
            //    options.LoginPath = new PathString("/Fourth/Login");// 这里指定如果验证不通过就跳转到这个页面中去
            //    options.ClaimsIssuer = "Cookie";
            //});
            #endregion


            services.AddSession();
            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(CustomExceptionFilterAttribute));// 这里就是全局注册Filter  
                //o.Filters.Add(typeof(CutomAuthorizeAttribute));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            //允许使用ServiceFilter 标记特性
            services.AddScoped<CustomActionFilterAttribute>();
            // 实例一个容器
            ContainerBuilder containerbuilder = new ContainerBuilder();

            #region 注册服务  非配置文件注册服务 
            containerbuilder.RegisterModule<CustomAutofacModule>();
            #endregion

            containerbuilder.Populate(services); // autofac 全权接管了之前这个Service的所有工作 
            IContainer container = containerbuilder.Build();
            return new AutofacServiceProvider(container);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
        {
            ILogger<Startup> _ilogger = factory.CreateLogger<Startup>();
            _ilogger.LogError($"this Startup Error");

            //#region 认证授权基本模型
            ////// 登录
            //app.Map("/login", builder => builder.Use(next =>
            //{
            //    return async (context) =>
            //    {
            //        var claimIdentity = new ClaimsIdentity();  // 可以把这个ClaimsIdentity理解成一个身份证
            //        claimIdentity.AddClaim(new Claim(ClaimTypes.Name, "Richard")); // 用户信息的载体
            //        // Scheme 可以理解为 跟身份证对应的一个标识
            //        await context.SignInAsync("myScheme", new ClaimsPrincipal(claimIdentity));
            //        await context.Response.WriteAsync($"Hello, ASP.NET Core! {context.User.Identity.Name} Login");
            //    };
            //}));
            //// 退出
            //app.Map("/logout", builder => builder.Use(next =>
            //{
            //    return async (context) =>
            //    {
            //        await context.SignOutAsync("myScheme");
            //    };
            //}));

            //app.Use(next => //认证   认识身份证
            //{
            //    return async (context) =>
            //    {
            //        var result = await context.AuthenticateAsync("myScheme");
            //        if (result?.Principal != null)
            //        {
            //            context.User = result.Principal;
            //            await next(context);
            //        }
            //        else
            //        {
            //            await context.ChallengeAsync("myScheme");
            //        }
            //    };
            //});
            //// 授权 身份证有什么权限
            //app.Use(async (context, next) =>
            //{
            //    var user = context.User;
            //    if (user.Identity.Name.Equals("Richard"))//user?.Identity?.IsAuthenticated这里没有授权检测 只检查下名称
            //    {
            //        await next();
            //    }
            //    else
            //    {
            //        await context.ForbidAsync("myScheme");
            //    }
            //    //if (user?.Identity?.IsAuthenticated ?? false)
            //    //{
            //    //    if (user.Identity.Name != "jim") await context.ForbidAsync("eScheme");
            //    //    else await next();
            //    //}
            //    //else
            //    //{
            //    //    await context.ChallengeAsync("eScheme");
            //    //}
            //});

            //// 访问受保护资源
            //app.Map("/resource", builder => builder.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync($"Hello, ASP.NET Core! {context.User.Identity.Name}");
            //}));

            //app.Run(async (HttpContext context) =>
            //{
            //    await context.Response.WriteAsync("Hello World,success！");
            //});
            //#endregion

            #region MiddleWare 
            //AuthenticationHttpContextExtensions 
            /*   app.Run(c => c.Response.WriteAsync("Hello World!")); // 终结*/

            //1 Run 终结式  只是执行，没有去调用Next  
            //一般作为终结点
            //app.Run(async (HttpContext context) =>
            //{
            //    await context.Response.WriteAsync("Hello World Run");  // 没有next  没有去执行下一个中间件
            //});

            //app.Run(async (HttpContext context) =>
            //{
            //    await context.Response.WriteAsync("Hello World Run Again");
            //}); 
            //app.Use(next =>   // next 是一个返回值  作为下一个中间件的一个参数
            //{
            //    Console.WriteLine("this is Middleware1");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware1 start</h3>");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("<h3>This is Middleware1 end</h3>");
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("this is Middleware2");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware2 start</h3>");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("<h3>This is Middleware2 end</h3>");
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("this is Middleware2");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("<h3>This is Middleware3 start</h3>");
            //        //await next.Invoke(context);
            //        await context.Response.WriteAsync("<h3>This is Middleware3 end</h3>");
            //    });
            //}); 

            //2 Use表示注册动作 不是终结点
            //执行next，就可以执行下一个中间件 如果不执行，就等于Run
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello World Use1 <br/>");
            //    await next();
            //    await context.Response.WriteAsync("Hello World Use1 End <br/>");
            //});
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello World Use2 Again <br/>");
            //    await next();
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
            //        await next();
            //    });
            //});

            //app.Use(async (context, next) =>//没有调用 next() 那就是终结点  跟Run一样
            //{ 
            //    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            //    //await next();
            //});

            // Map：根据条件指定中间件 指向终结点，没有Next
            //最好不要在中间件里面判断条件选择分支；而是一个中间件只做一件事儿，多件事儿就多个中间件
            //app.Map("/Test", MapTest);
            //app.Map("/Richard", a => a.Run(async context =>
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


            ////Middleware类
            //app.UseMiddleware<FirstMiddleWare>();
            //app.UseMiddleware<SecondMiddleWare>();
            //app.UseMiddleware<ThreeMiddleWare>();
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            //});
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy(); //
            app.UseAuthentication(); // 标识在当前系统中使用这个权限认证
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void MapTest(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync($"Url is {context.Request.Path.Value}");
            });
        }
    }
}