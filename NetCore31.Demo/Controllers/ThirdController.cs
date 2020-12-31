using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore31.Demo.Utility;
using NetCore31.Interface;

namespace NetCore31.Demo.Controllers
{
    /// <summary>
    /// 各种Filter研究
    /// Asp.NetCore-AOP-Filter
    /// 面向切面编程--做一些面向对象做不到的事情
    /// </summary>
    [CustomControllerFilter]
    //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]//控制器生效 需要ConfigureServices注入services.AddScoped(typeof(CustomExceptionFilterAttribute));
    public class ThirdController : Controller
    {
        private readonly ILogger<ThirdController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceA _serviceA;
        private readonly IServiceB _serviceB;
        private readonly IServiceC _serviceC;
        private readonly IServiceD _serviceD;
        private readonly IServiceE _serviceE;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        //private readonly IModelMetadataProvider _modelMetadataProvider;

        public ThirdController(ILogger<ThirdController> logger,
            ILoggerFactory loggerFactory,
            IServiceA serviceA,
            IServiceB serviceB,
            IServiceC serviceC,
            IServiceD serviceD,
            IServiceE serviceE,
            IServiceProvider serviceProvider,
            IConfiguration configuration
            //IModelMetadataProvider modelMetadataProvider
            )
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _serviceA = serviceA;
            _serviceB = serviceB;
            _serviceC = serviceC;
            _serviceD = serviceD;
            _serviceE = serviceE;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            // _modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// Action异常Filter
        /// </summary>
        /// <returns></returns>
        //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]//需要ConfigureServices注入services.AddScoped(typeof(CustomExceptionFilterAttribute));
        //[TypeFilter(typeof(CustomExceptionFilterAttribute))] //这种方式ConfigureServices中不需要注入
        [CustomIOCFilterFactory] //需要ConfigureServices注入services.AddScoped(typeof(CustomExceptionFilterAttribute));
        //[CustomExceptionFilterAttribute] //无法注入logger，modelMetadataProvider等其他信息
        public IActionResult Index()
        {
            //读取配置文件
            this._logger.LogWarning("This is Third Index");
            string allowHosts = _configuration["AllowedHosts"];
            string write = _configuration["ConnectionStrings:Write"];
            string read = _configuration["ConnectionStrings:Read:0"];
            string[] conn = _configuration.GetSection("ConnectionStrings").GetSection("Read").GetChildren().Select(x => x.Value).ToArray();
            //配置文件中不存在AllowedHost，所以调用ToString方法时会报错
            string allowHost = _configuration["AllowedHost"].ToString();//会异常
            return View();
        }

        /// <summary>
        /// 全局--控制器--Action Order默认为0，从小到大执行 可以是负数
        /// </summary>
        /// <returns></returns>
        [CustomActionFilter]
        [CustomActionCacheFilter]
        [CustomResourceFilter]
        public IActionResult Info()
        {
            Console.WriteLine($"This is {nameof(ThirdController)} Info");
            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }

        //那么时候时候使用中间件 什么时候使用Filter
        //--因为Filter是MVC的，中间件能知道action controller
        //--中间件是全部请求都要通过的，Filter可以针对Action/Controller
        //--粒度不同的，合适选择
    }
}