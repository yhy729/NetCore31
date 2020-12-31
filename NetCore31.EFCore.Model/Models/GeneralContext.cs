using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NetCore31.EFCore.Model.Models
{
    public partial class GeneralContext : DbContext
    {
        public GeneralContext()
        {
        }

        public GeneralContext(DbContextOptions<GeneralContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivityLog> ActivityLog { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<SysDomain> SysDomain { get; set; }
        public virtual DbSet<SysLog> SysLog { get; set; }
        public virtual DbSet<SysPermission> SysPermission { get; set; }
        public virtual DbSet<SysRole> SysRole { get; set; }
        public virtual DbSet<SysStore> SysStore { get; set; }
        public virtual DbSet<SysUser> SysUser { get; set; }
        public virtual DbSet<SysUserLoginLog> SysUserLoginLog { get; set; }
        public virtual DbSet<SysUserToken> SysUserToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;database=General;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Action)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Controller)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.CssClass)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.FatherId)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.FatherResource)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.ResourceId)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.RouteName)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.SysResource)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysDomain>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysLog>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysPermission>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysRole>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysStore>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Account).IsRequired();

                entity.Property(e => e.AllowLoginTime).HasColumnType("datetime");

                entity.Property(e => e.Avatar).HasColumnType("image");

                entity.Property(e => e.CreationTime).HasColumnType("datetime");

                entity.Property(e => e.DeleteTime).HasColumnType("datetime");

                entity.Property(e => e.LastActivityTime).HasColumnType("datetime");

                entity.Property(e => e.LastIpAddress).HasMaxLength(50);

                entity.Property(e => e.LastLoginTime).HasColumnType("datetime");

                entity.Property(e => e.MobilePhone).HasMaxLength(50);

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Sex).HasMaxLength(2);
            });

            modelBuilder.Entity<SysUserLoginLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IpAddress).HasMaxLength(50);

                entity.Property(e => e.LoginTime).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SysUserLoginLog)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_SysUserLoginLog_SysUser");
            });

            modelBuilder.Entity<SysUserToken>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ExpireTime).HasColumnType("datetime");

                entity.HasOne(d => d.SysUser)
                    .WithMany(p => p.SysUserToken)
                    .HasForeignKey(d => d.SysUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SysUserToken_SysUser");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
