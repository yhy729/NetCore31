using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore31.Demo.Models;
using NetCore31.Interface;
using NetCore31.Demo.Utility;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;

namespace NetCore31.Demo.Controllers
{
    public class FourthController : Controller
    {
        private readonly ILogger<FourthController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceA _serviceA;
        private readonly IServiceB _serviceB;
        private readonly IServiceC _serviceC;
        private readonly IServiceD _serviceD;
        private readonly IServiceE _serviceE;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public FourthController(ILogger<FourthController> logger,
            ILoggerFactory loggerFactory,
            IServiceA serviceA,
            IServiceB serviceB,
            IServiceC serviceC,
            IServiceD serviceD,
            IServiceE serviceE,
            IServiceProvider serviceProvider,
            IConfiguration configuration
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
        }

        //[TypeFilter(typeof(CustomActionCheckFilterAttribute))]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]//响应get请求
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        //[CustomAllowAnonymous]
        public ActionResult Login(string name, string password, string verify)
        {
            string verifyCode = base.HttpContext.Session.GetString("CheckCode");
            if (verifyCode != null && verifyCode.Equals(verify, StringComparison.CurrentCultureIgnoreCase))
            {
                if ("Yhy".Equals(name) && "123456".Equals(password))
                {
                    CurrentUser currentUser = new CurrentUser()
                    {
                        Id = 123,
                        Name = "admin",
                        Account = "admin666",
                        Email = "888888888",
                        Password = "123456",
                        LoginTime = DateTime.Now
                    };

                    #region Cookie/Session 自己写
                    //base.HttpContext.SetCookies("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser), 30);
                    //base.HttpContext.Session.SetString("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser));
                    #endregion
                    //过期时间全局设置

                    #region MyRegion
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,name),
                        new Claim("password",password),//可以写入任意数据
                        new Claim("Account","Administrator")
                    };
                    var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                    }).Wait();//没用await
                    ////cookie策略--用户信息---过期时间
                    #endregion

                    return base.Redirect("/Home/Index");
                }
                else
                {
                    base.ViewBag.Msg = "账号密码错误";
                }
            }
            else
            {
                base.ViewBag.Msg = "验证码错误";
            }
            return View();
        }

        public ActionResult VerifyCode()
        {
            string code = "";
            Bitmap bitmap = VerifyCodeHelper.CreateVerifyCode(out code);
            base.HttpContext.Session.SetString("CheckCode", code);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }

        [HttpPost]
        //[CustomAllowAnonymous]
        public ActionResult Logout()
        {
            #region Cookie
            base.HttpContext.Response.Cookies.Delete("CurrentUser");
            #endregion Cookie

            #region Session
            CurrentUser sessionUser = base.HttpContext.GetCurrentUserBySession();
            if (sessionUser != null)
            {
                this._logger.LogDebug(string.Format("用户id={0} Name={1}退出系统", sessionUser.Id, sessionUser.Name));
            }
            base.HttpContext.Session.Remove("CurrentUser");
            base.HttpContext.Session.Clear();
            #endregion Session

            #region MyRegion
            //HttpContext.User.Claims//其他信息
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            #endregion
            return RedirectToAction("Index", "Home"); ;
        }
    }
}