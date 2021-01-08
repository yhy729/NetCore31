using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore31.Demo.Utility;
using NetCore31.Interface;
using NetCore31.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NetCore31.Demo.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.FileProviders.Internal;
using NetCore31.Demo.Utility.MiddleWare;
using NetCore31.EFCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace NetCore31.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// 初始化，最早执行且执行一次的
        /// 给IOC容器增加映射关系
        /// IServiceCollection--容器
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            services.AddControllersWithViews(options =>
                     {
                         options.Filters.Add<CustomExceptionFilterAttribute>();//全局注册
                         options.Filters.Add<CustomGlobalFilterAttribute>();
                     });//.AddRazorRuntimeCompilation();//修改cshtml后能自动编译;

            //框架提供的将Controller注册为容器托管的服务
            //services.AddMvc().AddControllersAsServices();
            //services.AddMvcCore().AddControllersAsServices();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        //如果没有登录跳转的地址
                        options.LoginPath = new PathString("/Fourth/Login");
                        //如果没有权限跳转的地址
                        options.AccessDeniedPath = new PathString("/Home/Privacy");
                    });//用cookie的方式验证，顺便初始化登录地址

            #region 注入DbContext
            //直接注入(在MyDbContext中自己控制数据库连接)
            //EFCore官方的建议是使用Scoped的方式注入DbContext
            services.AddScoped<DbContext, MyDbContext>();

            //注入的同时传入options(注入的时候才指定数据库连接,这种方式配置在注入的地方需要注入的是MyDbContext而不能是DbContext)
            //services.AddDbContext<MyDbContext>(options =>
            //        {
            //            //读取配置文件中的链接字符串
            //            options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection"));
            //        });

            //注入的同时传入options(注入的时候才指定数据库连接,这种方式配置在注入的地方需要注入的是MyDbContext而不能是DbContext)
            //services.AddEntityFrameworkSqlServer()
            //        .AddDbContext<MyDbContext>(options =>
            //        {
            //            //读取配置文件中的链接字符串
            //            options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection"));
            //        }); 
            #endregion

            //不是直接new 而是容器生成 就可已自动注入
            services.AddScoped(typeof(CustomExceptionFilterAttribute));

            ////生命周期
            ////舜时
            //services.AddTransient<IServiceA, ServiceA>();
            ////单例
            //services.AddSingleton<IServiceB, ServiceB>();
            ////作用域单例--一次请求一个实例
            ////作用域其实依赖于ServiceProvider(这个自身是根据请求的)，跟多线程没有关系
            //services.AddScoped<IServiceC, ServiceC>();
            //services.AddTransient<IServiceD, ServiceD>();
            //services.AddTransient<IServiceE, ServiceE>();

            services.AddScoped<IUserService, UserService>();

            services.Configure<EmailOption>(op => op.Title = "Default Name");
            services.Configure<EmailOption>("FromMemory", op => op.Title = "FromMemory");
            services.Configure<EmailOption>("FromConfiguration", Configuration.GetSection("Email"));
            services.AddOptions<EmailOption>("AddOption").Configure(op => op.Title = "AddOption Title");

            services.Configure<EmailOption>(null, op => op.From = "Same With ConfigureAll");
            //services.ConfigureAll<EmailOption>(op => op.From = "ConfigureAll");

            services.PostConfigure<EmailOption>(null, op => op.Body = "Same With PostConfigureAll");
            //services.PostConfigureAll<EmailOption>(op => op.Body = "PostConfigurationAll");
        }

        /// <summary>
        /// Aotofac注入
        /// IServiceCollection注入和Aotofac注入可以同时使用
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CustomAutofacModule>();

            #region Controller托管到Autofac中并支持属性注入
            //var controllerBaseType = typeof(ControllerBase);
            ////扫描Controller类
            //containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
            //                .Where(x => controllerBaseType.IsAssignableFrom(x) && x != controllerBaseType)
            //                .PropertiesAutowired(); //支持属性注入 
            #endregion
        }

        /// <summary>
        /// Http请求管道模型
        /// 这个方法，叫请求级(所有请求生效)--页面级指Home/Index
        /// 这个方法执行且执行一次，是初始化
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var server = app.ApplicationServices.GetRequiredService<IServer>();
            var serverAddressesFeature = server.Features?.Get<IServerAddressesFeature>();
            var addresses = serverAddressesFeature?.Addresses;
            var webHostDefaults = WebHostDefaults.ServerUrlsKey;
            var hostdefault = HostDefaults.ApplicationKey;

            //任何请求都是响应这个
            //中断式中间件，直接停止了流程
            //app.Run(context => context.Response.WriteAsync("Hello World!"));

            #region Use中间件
            //RequestDelegate 是一个委托，接受一个HttpContex，执行操作，然后没有然后了
            //IApplicationBuilder在Build之后，就是一个RequestDelegate
            //所谓管道，其实就应该是个RequestDelegate

            //Func<RequestDelegate, RequestDelegate> func = new Func<RequestDelegate, RequestDelegate>(rq =>
            //{
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World 1!");
            //    });
            //});
            //app.Use(func);

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware1");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World 1 Start!");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("Hello World 1 End!");
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware1.5");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World 1.5 Start!");
            //        await next.Invoke(context);
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware1.6");
            //    return new RequestDelegate(async context =>
            //    {
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("Hello World 1.6 End!");
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware2");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World 2 Start!");
            //        await next.Invoke(context);
            //        await context.Response.WriteAsync("Hello World 2 End!");
            //    });
            //});

            ////注意 response has been started
            ////因为response-content length-不允许写,可以使用OnStarting等方式实现部分需求
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware2.5");
            //    return new RequestDelegate(async context =>
            //    {
            //        //await context.Response.WriteAsync("Hello World 2.5 Start!");
            //        context.Response.OnStarting(state =>
            //        {
            //            var httpContext = (HttpContext)state;
            //            httpContext.Response.Headers.Add("middleware", "123456");
            //            return Task.CompletedTask;
            //        }, context);
            //        await next.Invoke(context);
            //        //await context.Response.WriteAsync("Hello World 2.5 End!");
            //        await Task.Run(() => Console.WriteLine("01234567890"));
            //    });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware3");
            //    return new RequestDelegate(async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World 3 Start!");
            //        //await next.Invoke(context); //最后这个没有执行next
            //        await context.Response.WriteAsync("Hello World 3 End!");
            //    });
            //});

            #region ApplicationBuilder Build源码
            /*
             * public RequestDelegate Build()
               {
                   RequestDelegate app = context =>
                   {
                       // If we reach the end of the pipeline, but we have an endpoint, then something unexpected has happened.
                       // This could happen if user code sets an endpoint, but they forgot to add the UseEndpoint middleware.
                       var endpoint = context.GetEndpoint();
                       var endpointRequestDelegate = endpoint?.RequestDelegate;
                       if (endpointRequestDelegate != null)
                       {
                           var message =
                               $"The request reached the end of the pipeline without executing the endpoint: '{endpoint!.DisplayName}'. " +
                               $"Please register the EndpointMiddleware using '{nameof(IApplicationBuilder)}.UseEndpoints(...)' if using " +
                               $"routing.";
                           throw new InvalidOperationException(message);
                       }

                       context.Response.StatusCode = StatusCodes.Status404NotFound;
                       return Task.CompletedTask;
                   };

                   for (var c = _components.Count - 1; c >= 0; c--)
                   {
                       app = _components[c](app);
                   }

                   return app;
               }
             */
            #endregion
            #endregion

            #region Middleware
            //app.UseWhen(
            //       context =>
            //       {
            //           return context.Request.Query.ContainsKey("Name");
            //       },
            //       appBuilder =>
            //       {
            //           appBuilder.Use(async (context, next) =>
            //           {
            //               await context.Response.WriteAsync("Hello world UseWhen <br/>");
            //           });
            //       });

            //app.Map("Test",)
            //app.MapWhen();

            //UseMiddleware类
            //app.UseMiddleware<FirstMIddleWare>();
            #endregion

            #region 环境参数
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            #endregion

            #region 这些是中间件 最终把请求交给MVC
            //第二种配置日志的方式
            //Configure添加ILoggerFactory参数,然后AddLog4Net
            loggerFactory.AddLog4Net();

            app.UseSession();

            //app.UseHttpsRedirection();

            //配置静态文件
            app.UseStaticFiles(new StaticFileOptions
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
        }
    }
}
