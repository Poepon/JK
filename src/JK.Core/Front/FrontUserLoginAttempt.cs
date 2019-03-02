using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;

namespace JK
{
    public abstract class FrontUserLoginAttempt : Entity<long>, IHasCreationTime, IMustHaveTenant
    {
        /// <summary>
        /// Max length of the <see cref="TenancyName"/> property.
        /// </summary>
        public const int MaxTenancyNameLength = AbpTenantBase.MaxTenancyNameLength;

        /// <summary>
        /// Max length of the <see cref="TenancyName"/> property.
        /// </summary>
        public const int MaxUserNameLength = 255;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public const int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public const int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public const int MaxBrowserInfoLength = 512;

        /// <summary>
        /// Tenant's Id, if <see cref="TenancyName"/> was a valid tenant name.
        /// </summary>
        public virtual int TenantId { get; set; }

        /// <summary>
        /// Tenancy name.
        /// </summary>
        [Required]
        [StringLength(MaxTenancyNameLength)]
        public virtual string TenancyName { get; set; }

        /// <summary>
        /// User's Id, if <see cref="UserName"/> was a valid username.
        /// </summary>
        public virtual long? CustomerId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [StringLength(MaxUserNameLength)]
        public virtual string UserName { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        [StringLength(MaxClientNameLength)]
        public virtual string ClientName { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        [StringLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// Login attempt result.
        /// </summary>
        public virtual AbpLoginResultType Result { get; set; }

        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerLoginAttempt"/> class.
        /// </summary>
        public FrontUserLoginAttempt()
        {
            CreationTime = Clock.Now;
        }
    }
}
