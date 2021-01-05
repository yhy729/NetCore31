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
         * 七大类：1:HttpContext 2:RequestDelegate 3:Middleware 4:ApplicationBuilder(IApplicationBuilder)
         *        5:Server(IServer) 6:WebHost(IWebHost) 7:WebHostBuilder
         * Pipeline = Server + Middlewares,
         * 我们可以将多个Middleware构建成一个单一的“HttpHandler”,
         * 那么整个ASP.NET Core框架将具有更加简单的表达：
         * Pipeline = Server + HttpHandler。
         */
        //RequestDelegate理解
        //Action<HttpContext>
        //Func<HttpContext,Task>
        //delegate Task RequestDelegate (HttpContext context)

        //Middleware理解
        //对于管道的中的某一个中间件来说，由后续中间件组成的管道体现为一个RequestDelegate对象，
        //由于当前中间件在完成了自身的请求处理任务之后，往往需要将请求分发给后续中间件进行处理，所有它它需要将由后续中间件构成的RequestDelegate作为输入。
        //Func<RequestDelegate, RequestDelegate>
        //控制台
        public static void Main(string[] args)
        {
            //new WebHostBuilder().UseKestrel()
            //                    .Configure(app => app.Run(context => context.Response.WriteAsync("Hello NetCore31")))
            //                    .Build()
            //                    .Run();
            //启动kestrel
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //指定kestrel
            Host.CreateDefaultBuilder(args)
                //第一种配置日志的方式
                //.ConfigureLogging(loggingBuilder =>
                // {
                //     //需要配置文件
                //     loggingBuilder.AddLog4Net();
                // })
                //配置Autofac容器 设置工厂来替换容器(只需要在这里设置ServiceProviderFactory为AutofacServiceProviderFactory即可)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>//开始IWebHostBuilder
                {
                    //Startup串起MVC
                    webBuilder.UseStartup<Startup>();
                });
    }
}
