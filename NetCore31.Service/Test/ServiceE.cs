﻿using NetCore31.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore31.Service
{
    public class ServiceE : IServiceE
    {
        public ServiceE(IServiceC serviceC)
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
        }

        public void Show()
        {
            Console.WriteLine("This is ServiceE E123456");
        }
    }
}
