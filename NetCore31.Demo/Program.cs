using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCore31.Demo
{
    public class Program
    {
        /*https://www.cnblogs.com/artech/p/inside-asp-net-core-framework.html
         * �ߴ��ࣺ1:HttpContext 2:RequestDelegate 3:Middleware 4:ApplicationBuilder(IApplicationBuilder)
         *        5:Server(IServer) 6:WebHost(IWebHost) 7:WebHostBuilder
         * Pipeline = Server + Middlewares,
         * ���ǿ��Խ����Middleware������һ����һ�ġ�HttpHandler��,
         * ��ô����ASP.NET Core��ܽ����и��Ӽ򵥵ı�
         * Pipeline = Server + HttpHandler��
         */
        //RequestDelegate���
        //Action<HttpContext>
        //Func<HttpContext,Task>
        //delegate Task RequestDelegate (HttpContext context)

        //Middleware���
        //���ڹܵ����е�ĳһ���м����˵���ɺ����м����ɵĹܵ�����Ϊһ��RequestDelegate����
        //���ڵ�ǰ�м������������������������֮��������Ҫ������ַ��������м�����д�������������Ҫ���ɺ����м�����ɵ�RequestDelegate��Ϊ���롣
        //Func<RequestDelegate, RequestDelegate>
        //����̨
        public static void Main(string[] args)
        {
            //new WebHostBuilder().UseKestrel()
            //                    .Configure(app => app.Run(context => context.Response.WriteAsync("Hello NetCore31")))
            //                    .Build()
            //                    .Run();
            //����kestrel
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //ָ��kestrel
            Host.CreateDefaultBuilder(args)
                //��һ��������־�ķ�ʽ
                //.ConfigureLogging(loggingBuilder =>
                // {
                //     //��Ҫ�����ļ�
                //     loggingBuilder.AddLog4Net();
                // })
                //����Autofac���� ���ù������滻����(ֻ��Ҫ����������ServiceProviderFactoryΪAutofacServiceProviderFactory����)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>//��ʼIWebHostBuilder
                {
                    //Startup����MVC
                    webBuilder.UseStartup<Startup>();
                });
    }
}
