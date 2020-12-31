using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility
{
    /// <summary>
    /// 基于完成Filter的依赖注入
    /// IFilterFactory就是Filter的工厂，任何环节都可以永工厂代替Filter
    /// </summary>
    public class CustomIOCFilterFactoryAttribute : Attribute, IFilterFactory
    {
        private readonly Type _filterType = null;

        public CustomIOCFilterFactoryAttribute(Type type)
        {
            this._filterType = type;
        }
        public CustomIOCFilterFactoryAttribute()
        {
            this._filterType = _filterType ?? typeof(CustomExceptionFilterAttribute);
        }

        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //从容器中获取-所以需要提前注册
            return (IFilterMetadata)serviceProvider.GetService(this._filterType);
            //return (IFilterMetadata)serviceProvider.GetService(typeof(CustomExceptionFilterAttribute));
        }
    }
}
