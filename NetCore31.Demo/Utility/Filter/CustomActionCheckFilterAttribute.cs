using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCore31.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility
{
    public class CustomActionCheckFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<CustomActionCheckFilterAttribute> _logger;
        public CustomActionCheckFilterAttribute(ILogger<CustomActionCheckFilterAttribute> logger
            )
        {
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CurrentUser currentUser = context.HttpContext.GetCurrentUserBySession();
            if (currentUser == null)
            {
                if (this.IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new
                    {
                        result = false,
                        msg = "请登录"
                    });
                }
                else
                {
                    context.Result = new RedirectResult("~/Fourth/Login");
                }
            }
            else
            {
                this._logger.LogDebug($"{currentUser.Name} 访问系统");
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
