using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.MultiTenancy;
using JK.Storage;
using JK.Chat;
using JK.Customers;
using JK.Alliance;

namespace JK.EntityFrameworkCore
{
    public class JKDbContext : AbpZeroDbContext<Tenant, Role, User, JKDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        #region Chat

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<ChatSession> ChatSession { get; set; }

        public virtual DbSet<ChatSessionMember> ChatSessionMembers { get; set; }

        public virtual DbSet<UserChatMessageLog> UserChatMessageLogs { get; set; }


        #endregion

        #region Customers

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<CustomerClaim> CustomerClaims { get; set; }

        public virtual DbSet<CustomerLogin> CustomerLogins { get; set; }

        public virtual DbSet<CustomerLoginAttempt> CustomerLoginAttempts { get; set; }

        public virtual DbSet<CustomerToken> CustomerTokens { get; set; }

        #endregion

        #region Alliance

        public virtual DbSet<Agent> Agents { get; set; }

        public virtual DbSet<AgentClaim> AgentClaims { get; set; }

        public virtual DbSet<AgentLogin> AgentLogins { get; set; }

        public virtual DbSet<AgentToken> AgentTokens { get; set; }

        public virtual DbSet<AgentLoginAttempt> AgentLoginAttempts { get; set; }

        public virtual DbSet<AgentRelationship> AgentRelationships { get; set; }


        #endregion

        #region Tenant

        public virtual DbSet<TenantDomain> TenantDomains { get; set; }

        #endregion

        public JKDbContext(DbContextOptions<JKDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserChatMessageLog>().HasIndex(log => new { log.SessionId, log.UserId });
            modelBuilder.Entity<ChatSessionMember>().HasIndex(member => member.SessionId);
            modelBuilder.Entity<ChatMessage>().HasIndex(msg => msg.SessionId);
        }
    }
}
