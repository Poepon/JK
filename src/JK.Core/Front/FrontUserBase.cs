using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace JK.Front
{
    public abstract class FrontUserBase<TUserLogin, TUserClaim,TUserToken>
        : FullAuditedEntity<long>, IMustHaveTenant, IPassivable
    {
        public const int MaxPlainPasswordLength = 32;
        public const int MaxUserNameLength = 256;
        public const int MaxPasswordLength = 128;
        [Required]
        [StringLength(MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(MaxPasswordLength)]
        public string Password { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int TenantId { get; set; }

        //
        // 摘要:
        //     Gets or sets the date and time, in UTC, when any customer lockout ends.
        //
        // 备注:
        //     A value in the past means the customer is not locked out.
        public virtual DateTimeOffset? LockoutEnd { get; set; }
        //
        // 摘要:
        //     Gets or sets a flag indicating if two factor authentication is enabled for this
        //     customer.
        public virtual bool IsTwoFactorEnabled { get; set; }
        //
        // 摘要:
        //     Gets or sets a flag indicating if a customer has confirmed their telephone address.
        public virtual bool IsPhoneNumberConfirmed { get; set; }
        //
        // 摘要:
        //     Gets or sets a telephone number for the customer.
        public virtual string PhoneNumber { get; set; }
        //
        // 摘要:
        //     A random value that must change whenever a customer is persisted to the store
        public virtual string ConcurrencyStamp { get; set; }
        //
        // 摘要:
        //     A random value that must change whenever a users credentials change (password
        //     changed, login removed)
        public virtual string SecurityStamp { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if a customer has confirmed their email address.
        public virtual bool IsEmailConfirmed { get; set; }
        //
        // 摘要:
        //     Gets or sets the normalized email address for this customer.
        public virtual string NormalizedEmail { get; set; }
        //
        // 摘要:
        //     Gets or sets the email address for this customer.
        public virtual string Email { get; set; }
        //
        // 摘要:
        //     Gets or sets the normalized user name for this customer.
        public virtual string NormalizedUserName { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if the customer could be locked out.
        public virtual bool IsLockoutEnabled { get; set; }
        //
        // 摘要:
        //     Gets or sets the number of failed login attempts for the current customer.
        public virtual int AccessFailedCount { get; set; }

        public abstract ICollection<TUserLogin> Logins { get; set; }

        public abstract ICollection<TUserClaim> Claims { get; set; }

        public abstract ICollection<TUserToken> Tokens { get; set; }
    }
}
