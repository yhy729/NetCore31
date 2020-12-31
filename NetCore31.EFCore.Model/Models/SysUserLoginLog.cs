using System;
using System.Collections.Generic;

namespace NetCore31.EFCore.Model.Models
{
    public partial class SysUserLoginLog
    {
        public Guid? UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime? LoginTime { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }

        public virtual SysUser User { get; set; }
    }
}
