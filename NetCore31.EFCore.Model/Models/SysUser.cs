using System;
using System.Collections.Generic;

namespace NetCore31.EFCore.Model.Models
{
    public partial class SysUser
    {
        public SysUser()
        {
            SysUserLoginLog = new HashSet<SysUserLoginLog>();
            SysUserToken = new HashSet<SysUserToken>();
        }

        public string Account { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Sex { get; set; }
        public bool? Enabled { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? CreationTime { get; set; }
        public int? LoginFailedNum { get; set; }
        public DateTime? AllowLoginTime { get; set; }
        public bool? LoginLock { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime? LastActivityTime { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeleteTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public Guid? Modifier { get; set; }
        public Guid? Creator { get; set; }
        public byte[] Avatar { get; set; }

        public virtual ICollection<SysUserLoginLog> SysUserLoginLog { get; set; }
        public virtual ICollection<SysUserToken> SysUserToken { get; set; }
    }
}
