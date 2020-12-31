using NetCore31.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore31.Service
{
    public class ServiceB : IServiceB
    {
        public ServiceB(IServiceA iServiceA)
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
        }

        public void Show()
        {
            Console.WriteLine($"This is ServiceB B123456");
        }
    }
}
