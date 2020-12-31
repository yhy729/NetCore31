using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore31.EFCore.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCore31.EFCore.Model
{
    public partial class MyDbContext : DbContext
    {
        //public MyDbContext()
        //{
        //    Console.WriteLine("This is MyDbContext");
        //}

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Console.WriteLine("This is MyDbContext DbContextOptions");
        }

        //private IConfiguration _IConfiguration;
        //private ILoggerFactory _iLoggerFactory;

        //public MyDbContext(IConfiguration configuration, ILoggerFactory loggerFactory)
        //{
        //    this._IConfiguration = configuration;
        //    this._iLoggerFactory = loggerFactory;
        //    Console.WriteLine("This is MyDbContext configuration loggerFactory");
        //}


        public virtual DbSet<SysUser> SysUsers { set; get; }
        public virtual DbSet<SysUserLoginLog> SysUserLoginLogs { set; get; }
        public virtual DbSet<SysUserToken> SysUserTokens { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
