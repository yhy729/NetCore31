using System;
using System.Collections.Generic;

namespace NetCore31.EFCore.Model.Models
{
    public partial class SysUserToken
    {
        public Guid SysUserId { get; set; }
        public DateTime ExpireTime { get; set; }
        public Guid Id { get; set; }

        public virtual SysUser SysUser { get; set; }
    }
}
