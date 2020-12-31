using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.WebApi.Utility.Filter
{
    public class CustomResourceFilterAttribute : Attribute, IResourceFilter
    {
        private static Dictionary<string, object> dicCache = new Dictionary<string, object>();
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var key = context.HttpContext.Request.Path.ToString();
            if (dicCache.ContainsKey(key))
            {
                context.Result = dicCache[key] as ObjectResult;
            }
            Console.WriteLine($"this is {this.GetType().Name} OnResourceExecuting");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var key = context.HttpContext.Request.Path.ToString();
            var result = context.Result as ObjectResult;
            dicCache.Add(key, result);
            Console.WriteLine($"this is {this.GetType().Name} OnResourceExecuted");
        }
    }
}
