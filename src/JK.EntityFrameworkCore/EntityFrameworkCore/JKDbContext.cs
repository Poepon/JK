using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.MultiTenancy;
using JK.Storage;
using JK.Chat;
using JK.Customers;
using JK.Alliance;
using JK.Payments.Orders;
using JK.Payments.Bacis;
using JK.Payments.Integration;
using JK.Payments.TenantConfigs;

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

        #region Payments

        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }

        public virtual DbSet<PaymentOrder> PaymentOrders { get; set; }
        public virtual DbSet<PaymentOrderPolicy> PaymentOrderPolicies { get; set; }
        public virtual DbSet<PaymentOrderPolicyRule> PaymentOrderPolicyRules { get; set; }
        public virtual DbSet<PaymentOrderPolicyRuleValue> PaymentOrderPolicyRuleValues { get; set; }
        public virtual DbSet<PaymentOrderPolicyChannel> PaymentOrderPolicyChannels { get; set; }

        public virtual DbSet<TenantPaymentApp> TenantPaymentApps { get; set; }
        public virtual DbSet<TenantLimitPolicy> TenantLimitPolicies { get; set; }
        public virtual DbSet<TenantLimitPolicyRule> TenantLimitPolicyRules { get; set; }
        public virtual DbSet<TenantLimitPolicyRuleValue> TenantLimitPolicyRuleValues { get; set; }
        public virtual DbSet<CompanyAccount> CompanyAccounts { get; set; }

        public virtual DbSet<ApiParameter> ApiCallbackParameters { get; set; }
        public virtual DbSet<ApiChannel> ApiChannels { get; set; }
        public virtual DbSet<ApiConfiguration> ApiConfigurations { get; set; }
        public virtual DbSet<BankOverride> BankOverrides { get; set; }
        public virtual DbSet<ChannelOverride> ChannelOverrides { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyChannel> CompanyChannels { get; set; }
        public virtual DbSet<ResultCodeConfiguration> ResultCodeConfigurations { get; set; }
        public virtual DbSet<CompanyLimitPolicy> CompanyLimitPolicies { get; set; }
        public virtual DbSet<CompanyLimitPolicyRule> CompanyLimitPolicyRules { get; set; }
        public virtual DbSet<CompanyLimitPolicyRuleValue> CompanyLimitPolicyRuleValues { get; set; }

        public virtual DbSet<TenantPaymentAppChannel> TenantPaymentAppChannels { get; set; }
        public virtual DbSet<TenantPaymentAppCompany> TenantPaymentAppCompanies { get; set; }
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


            modelBuilder.Entity<ApiChannel>()
                .HasKey(t => new { t.ApiId, t.ChannelId });

            modelBuilder.Entity<ApiChannel>()
                .HasOne(pt => pt.Api)
                .WithMany(p => p.SupportedChannels)
                .HasForeignKey(pt => pt.ApiId);

            modelBuilder.Entity<ApiChannel>()
                .HasOne(pt => pt.Channel)
                .WithMany(t => t.ApiChannels)
                .HasForeignKey(pt => pt.ChannelId);

            modelBuilder.Entity<CompanyChannel>()
                .HasKey(t => new { t.CompanyId, t.ChannelId });

            modelBuilder.Entity<CompanyChannel>()
                .HasOne(pt => pt.Company)
                .WithMany(p => p.SupportedChannels)
                .HasForeignKey(pt => pt.CompanyId);

            modelBuilder.Entity<CompanyChannel>()
                .HasOne(pt => pt.Channel)
                .WithMany(t => t.CompanyChannels)
                .HasForeignKey(pt => pt.ChannelId);


            modelBuilder.Entity<PaymentOrderPolicyChannel>()
                .HasKey(t => new { t.PolicyId, t.ChannelId });

            modelBuilder.Entity<PaymentOrderPolicyChannel>()
                .HasOne(pt => pt.PaymentOrderPolicy)
                .WithMany(p => p.SupportedChannels)
                .HasForeignKey(pt => pt.PolicyId);

            modelBuilder.Entity<PaymentOrderPolicyChannel>()
                .HasOne(pt => pt.Channel)
                .WithMany(t => t.PaymentOrderPolicyChannels)
                .HasForeignKey(pt => pt.ChannelId);

            modelBuilder.Entity<TenantPaymentAppChannel>()
                .HasKey(t => new { t.AppId, t.ChannelId });

            modelBuilder.Entity<TenantPaymentAppChannel>()
                .HasOne(pt => pt.App)
                .WithMany(p => p.SupportedChannels)
                .HasForeignKey(pt => pt.AppId);

            modelBuilder.Entity<TenantPaymentAppChannel>()
                .HasOne(pt => pt.Channel)
                .WithMany(t => t.AppChannels)
                .HasForeignKey(pt => pt.ChannelId);

            modelBuilder.Entity<TenantPaymentAppCompany>()
                .HasKey(t => new { t.AppId, t.CompanyId });

            modelBuilder.Entity<TenantPaymentAppCompany>()
                .HasOne(pt => pt.App)
                .WithMany(p => p.SupportedCompanies)
                .HasForeignKey(pt => pt.AppId);

            modelBuilder.Entity<TenantPaymentAppCompany>()
                .HasOne(pt => pt.Company)
                .WithMany(t => t.SupportedApps)
                .HasForeignKey(pt => pt.CompanyId);
        }
    }
}
