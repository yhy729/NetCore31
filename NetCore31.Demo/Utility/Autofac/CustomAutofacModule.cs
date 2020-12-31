using Autofac;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using NetCore31.Interface;
using NetCore31.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCore31.Demo.Utility
{
    public class CustomAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var builder = new ContainerBuilder();
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);
            builder.RegisterType<ApplicationPartManager>().AsSelf().SingleInstance();
            builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray()).PropertiesAutowired();
            //containerBuilder.RegisterType<FirstController>().PropertiesAutowired();

            //containerBuilder.Register(c => new CustomAutofacAop());//aop注册
            containerBuilder.RegisterType<ServiceA>().As<IServiceA>().SingleInstance().PropertiesAutowired();
            containerBuilder.RegisterType<ServiceC>().As<IServiceC>();
            containerBuilder.RegisterType<ServiceB>().As<IServiceB>();
            containerBuilder.RegisterType<ServiceD>().As<IServiceD>();
            containerBuilder.RegisterType<ServiceE>().As<IServiceE>();

            //containerBuilder.RegisterType<A>().As<IA>();//.EnableInterfaceInterceptors();

            //containerBuilder.Register<FirstController>();

            //containerBuilder.RegisterType<JDDbContext>().As<DbContext>();
            //containerBuilder.RegisterType<CategoryService>().As<ICategoryService>();

            //containerBuilder.RegisterType<UserServiceTest>().As<IUserServiceTest>();

        }
    }
}
