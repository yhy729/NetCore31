using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore31.Demo.Models;

namespace NetCore31.Demo.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class FirstController : Controller
    {
        //Logger注入
        private readonly ILogger<FirstController> _logger;

        //LoggerFactory注入
        private readonly ILoggerFactory _loggerFactory;

        IOptions<EmailOption> _options;

        IOptionsMonitor<EmailOption> _optionsMonitor;

        IOptionsSnapshot<EmailOption> _optionsSnapshot;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="options"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="optionsSnapshot"></param>
        public FirstController(ILogger<FirstController> logger,
                               ILoggerFactory loggerFactory,
                               IOptions<EmailOption> options,
                               IOptionsMonitor<EmailOption> optionsMonitor,
                               IOptionsSnapshot<EmailOption> optionsSnapshot)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _options = options;
            _optionsMonitor = optionsMonitor;
            _optionsSnapshot = optionsSnapshot;
        }

        /// <summary>
        /// 后端Action到页面传值的几种方式
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _logger.LogWarning("This is FirstController Index");
            _loggerFactory.CreateLogger<FirstController>().LogWarning("This is FirstController Index Factory");

            base.ViewBag.User1 = "111";
            base.ViewData["User2"] = "222";
            base.TempData["User3"] = "333";

            //要想获取Session信息，需要在管道启用Session 
            //app.UseSession();
            //services.AddSession();
            string result = base.HttpContext.Session.GetString("User4");
            if (string.IsNullOrWhiteSpace(result))
            {
                base.HttpContext.Session.SetString("User4", "情深深");
            }

            object name = "缘浅浅";

            return View(name);
        }

        /// <summary>
        /// 最佳实践
        ///既然有如此多的获取方式，那应该如何选择？
        ///如果TOption不需要监控且整个程序就只有一个同类型的TOption，那么强烈建议使用IOptions<TOptions>。
        ///如果TOption需要监控或者整个程序有多个同类型的TOption，那么只能选择IOptionsMonitor<TOptions> 或者IOptionsSnapshot<TOptions>。
        ///当IOptionsMonitor<TOptions> 和IOptionsSnapshot<TOptions>都可以选择时，如果Action<TOptions> 是一个比较耗时的操作，那么建议使用IOptionsMonitor<TOptions>，反之选择IOptionsSnapshot<TOptions>
        ///如果需要对配置源的更新做出反应时（不仅仅是配置对象TOptions本身的更新），那么只能使用IOptionsMonitor<TOptions>，并且注册回调。
        /// </summary>
        /// <returns></returns>
        public IActionResult Demo()
        {
            //OptionsManager;
            //IOptionsFactory;
            //OptionsFactory;
            //ConfigureNamedOptions;
            //ConfigureNamedOptions<>;
            //PostConfigureOptions;
            //ValidateOptions;
            //IOptionsMonitorCache;
            //OptionsCache;
            //IOptionsMonitor;
            //OptionsBuilder;
            EmailOption defaultEmailOption = _options.Value;

            EmailOption defaultEmailOption1 = _optionsMonitor.CurrentValue;//_optionsMonitor.Get(Microsoft.Extensions.Options.Options.DefaultName);
            EmailOption fromMemoryEmailOption1 = _optionsMonitor.Get("FromMemory");
            EmailOption fromConfigurationEmailOption1 = _optionsMonitor.Get("FromConfiguration");

            EmailOption defaultEmailOption2 = _optionsSnapshot.Value;//_optionsSnapshot.Get(Microsoft.Extensions.Options.Options.DefaultName);
            EmailOption fromMemoryEmailOption2 = _optionsSnapshot.Get("FromMemory");
            EmailOption fromConfigurationEmailOption2 = _optionsSnapshot.Get("FromConfiguration");
            return View();
        }
    }
}