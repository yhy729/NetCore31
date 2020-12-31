using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger
            , IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// 异常发生，但是没有处理时
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                _logger.LogError($"{context.HttpContext.Request.RouteValues["controller"]} is Error");
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    //中断式-请求到这里就结束了，不再继续执行Action
                    context.Result = new JsonResult(new
                    {
                        result = false,
                        msg = context.Exception.Message
                    });
                }
                else
                {
                    var result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                    };
                    result.ViewData.Add("Exception", context.Exception);
                    context.Result = result;
                }
            }
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// 判断是否为Ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool IsAjaxRequest(HttpRequest request)
        {
            var header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
