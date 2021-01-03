using Microsoft.EntityFrameworkCore;
using NetCore31.EFCore.Model.Models;
using NetCore31.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore31.Service
{
    public class UserService : BaseService, IUserService
    {
        public UserService(DbContext context) : base(context)
        {
        }

        public void UpdateLastLogin(SysUser user)
        {
            SysUser userDB = base.Find<SysUser>(user.Id);
            if (userDB != null)
            {
                userDB.LastLoginTime = DateTime.Now;
                this.Commit();
            }
        }
        //public void Add()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Query()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Update()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
