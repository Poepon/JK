using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.MultiTenancy;
using JK.Storage;
using JK.Chat;

namespace JK.EntityFrameworkCore
{
    public class JKDbContext : AbpZeroDbContext<Tenant, Role, User, JKDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        #region Chat

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<ChatGroup> ChatGroups { get; set; }

        public virtual DbSet<ChatGroupMember> ChatGroupMembers { get; set; }

        public virtual DbSet<UserChatMessageLog> UserChatMessageLogs { get; set; }


        #endregion

        public JKDbContext(DbContextOptions<JKDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserChatMessageLog>().HasIndex(log => new { log.GroupId, log.UserId });
            modelBuilder.Entity<ChatGroupMember>().HasIndex(member => member.GroupId);
            modelBuilder.Entity<ChatMessage>().HasIndex(msg => msg.GroupId);
        }
    }
}
