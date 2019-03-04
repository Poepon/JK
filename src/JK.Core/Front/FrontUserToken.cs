using Abp;
using Abp.Domain.Entities;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace JK
{
    public abstract class FrontUserToken : Entity<long>
    {
        public FrontUserToken() { }
        public FrontUserToken(int tenantId,long userId, [NotNull] string loginProvider, [NotNull] string name, string value, DateTime? expireDate = null)
        {
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(name, nameof(name));

            TenantId = tenantId;
            UserId = userId;
            LoginProvider = loginProvider;
            Name = name;
            Value = value;
            ExpireDate = expireDate;
        }
        /// <summary>
        /// Maximum length of the <see cref="LoginProvider"/> property.
        /// </summary>
        public const int MaxLoginProviderLength = 128;

        /// <summary>
        /// Maximum length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 128;

        /// <summary>
        /// Maximum length of the <see cref="Value"/> property.
        /// </summary>
        public const int MaxValueLength = 512;

        public virtual int TenantId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public abstract long UserId { get; set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        [StringLength(MaxValueLength)]
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the token expire date
        /// </summary>
        public DateTime? ExpireDate { get; set; }
    }
}
