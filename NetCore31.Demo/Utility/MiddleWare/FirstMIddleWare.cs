using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility.MiddleWare
{
    public class FirstMIddleWare
    {
        private readonly RequestDelegate _next;

        public FirstMIddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync($"{nameof(FirstMIddleWare)},Hello Word Start! <br/>");
            await _next(context);
            await context.Response.WriteAsync($"{nameof(FirstMIddleWare)},Hello Word End! <br/>");
        }
    }
}
