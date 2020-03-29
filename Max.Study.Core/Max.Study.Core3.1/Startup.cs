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
        /// ��������
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           // StaticConstraint.Init(s => configuration[s]);
        }

        public IConfiguration Configuration { get; }


        /// <summary>
        /// ��ʼ��������ִ����ִֻ��һ�ε�
        /// ��IOC��������ӳ���ϵ��
        /// IServiceCollection--����--�������DI--
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            //services.AddControllersWithViews();
            //services.AddRazorPages();//Լ����AddMvc() ����3.0�����ݲ�ֵĸ�ϸһЩ���ܸ�С������
            services.AddControllersWithViews(
                 options =>
                 {
                     options.Filters.Add<CustomExceptionFilterAttribute>();//ȫ��ע��
                    options.Filters.Add<CustomGlobalFilterAttribute>();
                 }).AddRazorRuntimeCompilation();//�޸�cshtml�����Զ�����
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Fourth/Login");
                    options.AccessDeniedPath = new PathString("/Home/Privacy");

                });//��cookie�ķ�ʽ��֤��˳���ʼ����¼��ַ

            //services.AddScoped<DbContext, JDDbContext>();//�������׻����ǰ��ҷ�װ��һ��

            services.AddScoped<IUserService, UserService>();


            //���ע��û�гɹ�--ע����û����ģ����캯��Ҳֻ��֧�ֲ����ͺã�����ע��ĵط�����дDbContext
            services.AddDbContext<JDDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("JDDbConnection"));
            });


            //services.AddEntityFrameworkSqlServer()
            //      .AddDbContext<JDDbContext>(options =>
            //      {
            //          options.UseSqlServer(Configuration.GetConnectionString("JDDbConnection") //��ȡ�����ļ��е������ַ���
            //              );  //���÷�ҳ ʹ�þɷ�ʽ
            //      });



            //services.AddScoped(typeof(CustomExceptionFilterAttribute));//����ֱ��new  ������������ �Ϳ����Զ�ע����

            //ֻ�ܹ��캯��ע��--��Ҫһ�����캯������
            //services.AddTransient<ITestServiceA, TestServiceA>();//˲ʱ
            //services.AddSingleton<ITestServiceB, TestServiceB>();//����
            //services.AddScoped<ITestServiceC, TestServiceC>();//��������--һ������һ��ʵ��
            ////��������ʵ������ServiceProvider����������Ǹ�������ģ��������߳�û��ϵ
            //services.AddTransient<ITestServiceD, TestServiceD>();
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CustomAutofacModule>();
        }

        /// <summary>
        /// Http����ܵ�ģ��---����Http���󱻴���Ĳ���---Http������һ���ı�����Kestrel�����õ�HttpContext---Ȼ�󱻺�̨���봦��Request---����Response---����Kestrel�ط����ͻ���
        /// ��ν�ܵ�����������HttpContext�������������ļӹ�������Response,���ǹܵ�
        /// Configure����ָ�����ǵĴ������ȥ��������
        /// 
        /// ���ɶҲ�����ã�����Ĭ���ǻ�����Ӧ����404
        /// ���������������(����������Ч)----ҳ�漶Home/Index
        /// �������ִ����ִֻ��һ�Σ��ǳ�ʼ��
        /// 
        /// RequestDelegate��һ��ί��,����һ��HttpContext��ִ�в�����Ȼ��û��Ȼ����
        /// IApplicationBuilder��Build֮�� ����һ��RequestDelegate
        /// Application--��ν�ܵ�����ʵ��Ӧ���Ǹ�RequestDelegate
        /// 
        /// ������һ�������������������̵�����ڵ�ȥ��չ
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

            #region Use�м��
            //app.Run(c => c.Response.WriteAsync("Hello World!"));
            //////�κ��������ˣ�ֻ�Ƿ��ظ�hello world    �ս�ʽ
            //////��νRun�ս�ʽע�ᣬ��ʵֻ��һ����չ���������ջ����ǵõ���Use������

            ////IApplicationBuilder Ӧ�ó������װ��
            ////RequestDelegate:����һ��HttpContext���첽�����£������أ�Ҳ����һ��������
            //// Use(Func<RequestDelegate, RequestDelegate> middleware) ί�У�����һ��RequestDelegate������һ��RequestDelegate
            ////ApplicationBuilder�����и�����IList<Func<RequestDelegate, RequestDelegate>> _components
            ////Use��ֻ��ȥ����������Ӹ�Ԫ��
            ////���ջ�Build()һ�£� ���û���κ�ע�ᣬ��ֱ��404����һ��
            ///*
            // foreach (var component in _components.Reverse())//��ת����  ÿ��ί���ó���
            //{
            //    app = component.Invoke(app);
            //    //ί��3-- 404��Ϊ�������ã����� ί��3�����ö���--��Ϊ����ȥ����ί��(��Ϊ��ί��2�Ĳ���)--ѭ����ȥ---���յõ�ί��1�����ö���---��������HttpContext---
            //}
            // */
            ////IApplicationBuilder build֮����ʵ����һ��RequestDelegate���ܶ�HttpContext���Դ���
            ////Ĭ������£��ܵ��ǿյģ�����404�����Ը�������������������ִ�У�һ��ȫ���ɿ��������ɶ��ƣ����ֻ���ṩ��һ����װ��ʽ

            ////Func<RequestDelegate, RequestDelegate> middleware = next =>
            ////{
            ////    return new RequestDelegate(async context =>
            ////                    {
            ////                        await context.Response.WriteAsync("<h3>This is Middleware1 start</h3>");
            ////                        await Task.CompletedTask;
            ////                        await next.Invoke(context);//RequestDelegate--��Ҫcontext����Task
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
            //        //await next.Invoke(context);//ע�͵�����ʾ����������
            //        await context.Response.WriteAsync("<h3>This is Middleware3 end</h3>");
            //    });
            //});

            //////1 Run �ս�ʽ  ֻ��ִ�У�û��ȥ����Next  
            //////һ����Ϊ�ս��
            ////app.Run(async (HttpContext context) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Run");
            ////});
            ////app.Run(async (HttpContext context) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Run Again");
            ////});

            //////2 Use��ʾע�ᶯ��  �����ս��  
            //////ִ��next���Ϳ���ִ����һ���м��   �����ִ�У��͵���Run
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
            //////UseWhen���Զ�HttpContext�������Ӵ�����;ԭ�������̻�������ִ�е�
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

            ////app.Use(async (context, next) =>//û�е��� next() �Ǿ����ս��  ��Runһ��
            ////{
            ////    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            ////    //await next();
            ////});

            //////Map����������ָ���м��  ָ���ս�㣬û��Next
            //////��ò�Ҫ���м�������ж�����ѡ���֧������һ���м��ֻ��һ���¶�������¶��Ͷ���м��
            ////app.Map("/Test", MapTest);
            ////app.Map("/Eleven", a => a.Run(async context =>
            ////{
            ////    await context.Response.WriteAsync($"This is Advanced Eleven Site");
            ////}));
            ////app.MapWhen(context =>
            ////{
            ////    return context.Request.Query.ContainsKey("Name");
            ////    //�ܾ���chorme�����������  
            ////    //������
            ////    //��ajaxͳһ����
            ////}, MapTest);

            ////Middleware��
            //app.UseMiddleware<ElevenStopMiddleware>();
            //app.UseMiddleware<ElevenMiddleware>();
            //app.UseMiddleware<ElevenSecondMiddleware>();
            ////app.Use(async (context, next) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            ////});
            #endregion

            #region middleware
            //Use�������Ǻܸ���
            //�ս�ʽ-- app.Use(_ => handler);---��ζ��û���¸�����
            //app.Run(c => c.Response.WriteAsync("Hello World!"));

            //�����use  ���Բ���request delegate
            //app.Use(async (context, next) =>//û�е��� next() �Ǿ����ս��  ��Runһ��
            //{
            //    await context.Response.WriteAsync("Hello World Use3  Again Again <br/>");
            //    //await next();
            //});
            //UseWhen���Զ�HttpContext�������Ӵ�����;ԭ�������̻�������ִ�е�
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

            ////��������ָ���м�� ָ���ս�㣬û��Next
            //app.Map("/Test", MapTest);
            //app.Map("/Eleven", a => a.Run(async context =>
            //{
            //    await context.Response.WriteAsync($"This is Advanced Eleven Site");
            //}));
            //app.MapWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //�ܾ���chorme�����������  
            //    //������
            //    //��ajaxͳһ����
            //}, MapTest);
            //���Ͼ�ΪUse�ķ�װ����ʵ��Ϊ����Ϥ���˷��㣬�����������Եĸ��Ӷ�

            ////UseMiddlerware ��--������
            //app.UseMiddleware<FirstMiddleWare>();
            //app.UseMiddleware<SecondMiddleWare>();
            //app.UseMiddleware<ThreeMiddleWare>();
            #endregion



            #region ��������
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

            #region ��Щ���м�������հ����󽻸�MVC
            loggerFactory.AddLog4Net();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });
            app.UseRouting();
            app.UseAuthentication();//��Ȩ�������û�е�¼����¼����˭����ֵ��User
            app.UseAuthorization();//������Ȩ�����Ȩ��
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
