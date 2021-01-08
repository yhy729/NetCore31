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
        /// ��ʼ��������ִ����ִ��һ�ε�
        /// ��IOC��������ӳ���ϵ
        /// IServiceCollection--����
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            services.AddControllersWithViews(options =>
                     {
                         options.Filters.Add<CustomExceptionFilterAttribute>();//ȫ��ע��
                         options.Filters.Add<CustomGlobalFilterAttribute>();
                     });//.AddRazorRuntimeCompilation();//�޸�cshtml�����Զ�����;

            //����ṩ�Ľ�Controllerע��Ϊ�����йܵķ���
            //services.AddMvc().AddControllersAsServices();
            //services.AddMvcCore().AddControllersAsServices();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        //���û�е�¼��ת�ĵ�ַ
                        options.LoginPath = new PathString("/Fourth/Login");
                        //���û��Ȩ����ת�ĵ�ַ
                        options.AccessDeniedPath = new PathString("/Home/Privacy");
                    });//��cookie�ķ�ʽ��֤��˳���ʼ����¼��ַ

            #region ע��DbContext
            //ֱ��ע��(��MyDbContext���Լ��������ݿ�����)
            //EFCore�ٷ��Ľ�����ʹ��Scoped�ķ�ʽע��DbContext
            services.AddScoped<DbContext, MyDbContext>();

            //ע���ͬʱ����options(ע���ʱ���ָ�����ݿ�����,���ַ�ʽ������ע��ĵط���Ҫע�����MyDbContext��������DbContext)
            //services.AddDbContext<MyDbContext>(options =>
            //        {
            //            //��ȡ�����ļ��е������ַ���
            //            options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection"));
            //        });

            //ע���ͬʱ����options(ע���ʱ���ָ�����ݿ�����,���ַ�ʽ������ע��ĵط���Ҫע�����MyDbContext��������DbContext)
            //services.AddEntityFrameworkSqlServer()
            //        .AddDbContext<MyDbContext>(options =>
            //        {
            //            //��ȡ�����ļ��е������ַ���
            //            options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection"));
            //        }); 
            #endregion

            //����ֱ��new ������������ �Ϳ����Զ�ע��
            services.AddScoped(typeof(CustomExceptionFilterAttribute));

            ////��������
            ////˴ʱ
            //services.AddTransient<IServiceA, ServiceA>();
            ////����
            //services.AddSingleton<IServiceB, ServiceB>();
            ////��������--һ������һ��ʵ��
            ////��������ʵ������ServiceProvider(��������Ǹ��������)�������߳�û�й�ϵ
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
        /// Aotofacע��
        /// IServiceCollectionע���Aotofacע�����ͬʱʹ��
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CustomAutofacModule>();

            #region Controller�йܵ�Autofac�в�֧������ע��
            //var controllerBaseType = typeof(ControllerBase);
            ////ɨ��Controller��
            //containerBuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
            //                .Where(x => controllerBaseType.IsAssignableFrom(x) && x != controllerBaseType)
            //                .PropertiesAutowired(); //֧������ע�� 
            #endregion
        }

        /// <summary>
        /// Http����ܵ�ģ��
        /// ���������������(����������Ч)--ҳ�漶ָHome/Index
        /// �������ִ����ִ��һ�Σ��ǳ�ʼ��
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

            //�κ���������Ӧ���
            //�ж�ʽ�м����ֱ��ֹͣ������
            //app.Run(context => context.Response.WriteAsync("Hello World!"));

            #region Use�м��
            //RequestDelegate ��һ��ί�У�����һ��HttpContex��ִ�в�����Ȼ��û��Ȼ����
            //IApplicationBuilder��Build֮�󣬾���һ��RequestDelegate
            //��ν�ܵ�����ʵ��Ӧ���Ǹ�RequestDelegate

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

            ////ע�� response has been started
            ////��Ϊresponse-content length-������д,����ʹ��OnStarting�ȷ�ʽʵ�ֲ�������
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
            //        //await next.Invoke(context); //������û��ִ��next
            //        await context.Response.WriteAsync("Hello World 3 End!");
            //    });
            //});

            #region ApplicationBuilder BuildԴ��
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

            //UseMiddleware��
            //app.UseMiddleware<FirstMIddleWare>();
            #endregion

            #region ��������
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            #endregion

            #region ��Щ���м�� ���հ����󽻸�MVC
            //�ڶ���������־�ķ�ʽ
            //Configure���ILoggerFactory����,Ȼ��AddLog4Net
            loggerFactory.AddLog4Net();

            app.UseSession();

            //app.UseHttpsRedirection();

            //���þ�̬�ļ�
            app.UseStaticFiles(new StaticFileOptions
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
        }
    }
}
