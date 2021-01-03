using NetCore31.EFCore.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore31.Interface
{
    public interface IUserService : IBaseService
    {
        //void Query();
        //void Update();
        //void Delete();
        //void Add();

        void UpdateLastLogin(SysUser user);
    }
}
