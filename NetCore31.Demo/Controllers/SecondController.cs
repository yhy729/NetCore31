using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCore31.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore31.Demo.Controllers
{
    public class SecondController : Controller
    {
        private readonly ILogger<SecondController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceA _serviceA;
        private readonly IServiceB _serviceB;
        private readonly IServiceC _serviceC;
        private readonly IServiceD _serviceD;
        private readonly IServiceE _serviceE;
        private readonly IServiceProvider _serviceProvider;

        public SecondController(ILogger<SecondController> logger,
            ILoggerFactory loggerFactory,
            IServiceA serviceA,
            IServiceB serviceB,
            IServiceC serviceC,
            IServiceD serviceD,
            IServiceE serviceE,
            IServiceProvider serviceProvider
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
        }

        public IActionResult Index()
        {
            _serviceA.Show();
            _serviceB.Show();
            _serviceC.Show();
            _serviceD.Show();
            _serviceE.Show();

            var sc = (IServiceC)_serviceProvider.GetService(typeof(IServiceC));
            var sc1 = _serviceProvider.GetService<IServiceC>();
            sc.Show();

            Console.WriteLine($"cc {object.ReferenceEquals(_serviceC, sc)}");

            _logger.LogWarning("This is SecondController Index");
            return View();
        }
    }
}