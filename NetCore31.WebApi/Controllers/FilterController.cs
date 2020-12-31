using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCore31.WebApi.Controllers
{
    //各种Filter研究
    /*
     * ResourceFilter(IResourceFilter）实例化Controller之前执行 适合做缓存
     * ActionFilter (ActionFilterAttribute,IActionFilter,IAsyncActionFilter) 实例化控制器之后 适合做性能监控 接口权限 写日志等
     * 
     * TypeFilter(支持注入，传入Type)
     * 
     * ServiceFilter(支持注入，传入Type 同时需要在Startup ConfigureServices注册该Filter)
     * ServiceFilter支持传入Order,从Order小到大执行
     * 
     * Global注册，Controller注册，Action注册
     * 
     * ExceptionFilter(IExceptionFilter)
     * 
     * ResultFilter(IResultFilter)
     */
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
    }
}