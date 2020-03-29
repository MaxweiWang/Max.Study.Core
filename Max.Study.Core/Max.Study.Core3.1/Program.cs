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
    /// 1 Asp.NetCore3.0Preview7�������ã���ĿǨ��
    /// 2 Razor��̬���룬TempData���л����������
    /// 3 �м��Դ����������¹ܵ�����ģ��
    /// 4 autofac��ģʽ
    /// 5 EntityFrameworkCore3.0ʹ��&��װ&��չ��־
    /// 6 .NetCore3.0 WebApi����Ӧ�ã�ǰ�����
    /// 
    /// �»������ã�Asp.NetCore3.0 Preview7
    /// ��ʱ���ʺ�ֱ��������Ŀ�����Կγ���Ҫѧϰ��2.2
    /// 1 ֻ����VS2019---VS2019���ص�ַ�ͼ�����
    /// 2 .NetCore3.0����Ҫ������װ����ʱ����CLR
    ///   dotnet-hosting-3.0.0-preview7.19365.7-win.exe
    ///   ���ﻹ����IIS��Ҫ��Module
    /// 3  SDK����������߰�--VS���ж�Ӧ��ģ��
    ///   dotnet-sdk-3.0.100-preview7-012821-win-x64-.exe
    ///   ����--ѡ��--����---Ԥ������--Preview
    /// 
    /// .NetCoreΪʲô���Կ�ƽ̨��
    /// ��Ϊ΢�����һ�׿�����Linux���е�CLR
    /// .NetFramework����CLR������һЩ��Core��CLR�仯�ܿ�
    /// 
    /// Core2.2����ʽ�棬���ڽ���3.0��Ԥ���棬��������ĸĶ�
    /// �ƻ�������ɴ�2.2��3.0������������仯������һ��ԭ��
    /// ����EFCore  Core��WebApi  �һ��3.0�Ŀ��
    /// ���⣬����һ��������
    /// ׼������ѧԱ����3.0�Ŀ�����һ����ʵ��Ŀ����ʦ��Ǯ(��ǧ)
    /// ����ʵ����Core������Ŀ��Ȼ����������
    /// 
    /// Razor cshtmlӦ�����ڷ����ǻᶯ̬����
    /// 3.0Ĭ����û�ж�̬����---nuget���AddRazorRuntimeCompilation--startup configservice�����--
    /// 
    /// tempdata���л�ʱ��ֻ���ǻ�������+int����+�ֵ䣬�������Զ�������
    /// 
    /// �ṩAspNetCore-masterԴ��.zip�ǿ���ֱ�Ӳ鿴Դ����
    /// Ҳ����ͨ��https://github.com/aspnet/AspNetCoreȥ�鿴
    /// 
    /// �������һ�£�
    /// MVC����������ͨ�������ռ�,���ﲻ��
    /// ��Ҫ�ڿ������������[Area("System")] [Route("System/[controller]/[action]")]   ÿ������������Ҫ�����Կ��Լ̳и�BaseAreaController
    /// 
    /// ��Core2.2����� ��Ǩ�Ƶ�Core3.0  �޸�targetframework����������
    /// 
    /// �ܵ�����ģ��--�м��
    /// Startup--Config����ȥָ����Http����ܵ�
    /// ��νhttp����Ĺܵ��أ�
    /// ���Ƕ�Http�����һ�����Ĵ������
    /// ���Ǹ���һ��HttpContext��Ȼ��һ�����Ĵ������յĵõ����
    /// 
    /// Asp.Net����ܵ��� �������ջ���һ�������HttpHandler����(page/ashx/mvchttphandler--action)
    ///                   ���ǻ��ж�����裬����װ���¼�--����ע�������չ--IHttpModule--�ṩ�˷ǳ��������չ��
    /// ��һ��ȱ�ݣ�̫������¶���--һ��http�����������IHttpHandler--cookie Session  Cache NeginRequest endrequest maprequesthandler ��Ȩ----��Щ��һ���ǵ���---����д����---Ĭ����Ϊ��Щ�����Ǳ����---����ܵ����˼���й�---.Net���ż򵥾�ͨ��---��Ϊ��ܴ��������ȫ��Ͱʽ�������һ�¿ؼ���д�����ݿ⣬һ����Ŀ�ͳ�����---���Ծ�ͨҲ��----ҲҪ�������ۣ����ǰ����Ƚ��أ�������װǰ��---.NetCore��һ��ȫ�µ�ƽ̨���Ѿ�������ǰ����---��Ƹ�׷���������׷�������---û��ȫ��Ͱ        
    /// 
    /// Asp.NetCoreȫ�µ�����ܵ���
    /// Ĭ��������ܵ�ֻ��һ��404
    /// Ȼ���������������Ĵ���(UseEndPoint)---�������ǰhandler��ֻ����ҵ����
    /// �����ľ����м��middleware
    /// 
    /// 
    /// 1 autofac��ģʽ
    /// 2 EntityFrameworkCore3.0ʹ��&��װ&��չ��־
    /// 3 .NetCore3.0 WebApi����Ӧ�ã�ǰ�����
    /// 
    /// �滻����ʱ��������
    /// a nuget--���Բο������������autofac���
    /// b UseServiceProviderFactory
    /// c ConfigureContainer(ContainerBuilder containerBuilder)
    /// 
    /// EntityFrameworkCore3.0
    /// û��edmx��һ����code first  Ҳû���Զ�����
    /// 
    /// ��nugetһ��efcore+efcoresqlserver
    /// a ��Framework����ʵ��+context��Ȼ����ճ��
    /// b JDDbContext���캯����ָ������
    /// c protected override void OnConfiguring �������
    /// d protected override void OnModelCreating���˸���������
    /// 
    /// �����������⣬�϶����������ļ��������ļ���ô��ȡ
    /// 1 �ڲ�д����appsetting--���� ·�����ܱ仯
    /// 2 dbcontextע��IConfiguration---û����
    /// 3 ���������ļ������Ҫ������������ǲ��Ƕ���ע��IConfiguration��
    ///   ����һЩ��̬������Ҫʹ�������ļ�
    ///   ��ǰ.NetFramework��������ļ����й������ɾ�̬��ʹ�õ�ʱ��ֱ���ã�
    ///   ����һ��StaticConstraint��Ȼ����startup��������ί����ɳ�ʼ��
    ///   
    /// .NetCore���ٳ��־�̬����Ҫ�Ļ���ͨ������ģʽ
    /// �Ϳ��Դ��ϵ��±���ȫ������ע��(���첻С)
    /// 
    /// WebApi:
    /// ֱ�������WebApi������,
    /// ���ﲻ��Ҫ��Ӷ����·�ɣ�ֱ�ӿ�����·��
    /// [Route("api/[controller]"), ApiController]  ��������
    /// ���⣬���Ǿ����action������Ҫ���������·��
    /// 
    /// btnGet5��ʱ��ʧ�ܣ���������о���   
    /// ˭�ȸ㶨�ģ�˽�ĸ���Ϊ׼��������Ⱥ�﷢�𰸣���ǰ3����8.88
    /// 
    /// WebApi�Ĺܵ���MVC�ϲ��ˣ���һ����
    /// Ȩ��Filter �쳣��Filter ��ͨ�õ�
    /// 
    /// .NetCore��ʵ���ѣ������������⣬���Ը㶨��
    /// ��ʵ����
    /// 
    /// 
    ///
    /// *********************΢����ܹ�****************************
    /// 1 ΢����ܹ���������ȱ�㡢��ս��ת��
    /// 2 ����ʵ��׼����Consul��װ
    /// 3 Consulע�ᣬ������⣬������
    /// 
    /// ΢����ܹ�ר�⣬����Core
    /// 
    /// �����в���--AddCommandLine---����ʱ���Դ��ݲ���---Ȼ�����Configuration["name"] ��ȡһ��
    /// 
    /// 
    /// ����ע���뷢��
    /// a  ���Webapi����---���log4net---ע�뵽������--��¼��־
    /// b  ��������webapi����---2��ʵ��--��ͬ�˿�
    /// c  ׼��consul--����--nuget-consul
    /// d  ��վ��������Ҫע�ᵽconsul
    /// e  ���health-check���������
    /// f  ��startupȥע����---Ȼ���������ʵ��
    /// g  ȥhttp://localhost:8500�鿴������񱻷��ֺ�������� 
    ///    
    /// 
    /// 1 Consul�����ߣ���ɸ��ؾ���
    /// 2 ����Ocelot�Gateway����ɷ������
    /// 3 Ocelot+Consul����ʹ�ã�����+����ע�ᷢ��
    /// 4 Ocelot���ý������۶�-�����ȹ���ʵ��
    ///
    /// ����Eleven��ʦ׼�������д�����ߣ�����Consul����ɷ������
    /// a ��׼���ٽ�����Ŀ�������ߣ�ֱ�Ӿ�д��һ����Ŀ
    /// 
    /// 
    /// Ocelot:
    /// Core-Webapi��Ŀ--����Ocelot---һ���м��---program�滻�����ļ�--���������ļ�--startupȥ����--ȫ������ocelot---�����ļ��������--����������ط���
    /// 
    /// 
    /// 1 Ocelot���ý��ף�����-����-�۶�-�ۺϵȹ���ʵ��
    /// 2 ���IdentityServer4��֤���ģ�OAuth-OpenID
    /// 3 �ֲ�ʽ���������CAP&Base����ģʽ����
    /// 4 Docker&K8S���ٷ�������
    /// 
    /// �����Ocelot+Consul����
    /// a  �ָ�����--Consul+2������ʵ��--���Ե���
    /// b  ocelot��������������ļ������Consul �������ת���Լ�������
    /// 
    /// ���ز������������ת��--�м�㣬�ҵĵ������ҵģ��Ҹ���showһ��
    /// a  ����---�����¾Ϳ�����
    /// b  ����---���Ƶ�λʱ�������������--������&��ɱ
    /// c  �۶�---���ﱣ��˿���ؾͻ��۶ϣ�ȫͣ�����Ա������˼������ĵ���
    ///           A�ɣ������˳���ƽ�����춯��ֱ��ͣ��ȫ������
    ///           ���ĳЩ�����󣬽ӿڽ���ʱ�����ṩ����
    ///    nuget--polly---AddPolly()
    /// d  ����ϲ�---�϶�����һ�£����ػ����������񣬽���ϲ�����get
    /// 
    /// Session---Token---OAuth+OpenId----IdentityServer4---AuthorizationServer---��ȡToken
    /// 
    /// ���촿����--�ֲ�ʽ����-Docker-K8S
    /// ��һ�����и��뷨���ƻ���Framework��Core����ʽѧϰ
    /// 
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddCommandLine(args)//֧��������
           .Build();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddFilter("System", LogLevel.Warning);
                    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//���˵�ϵͳĬ�ϵ�һЩ��־
                    loggingBuilder.AddLog4Net();//�ļ�·��
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())//���ù������滻ʵ��
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
