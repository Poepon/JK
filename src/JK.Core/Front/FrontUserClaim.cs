using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JK
{
    public abstract class FrontUserClaim : Entity<long>
    {
        /// <summary>
        /// Maximum length of the <see cref="ClaimType"/> property.
        /// </summary>
        public const int MaxClaimTypeLength = 256;

        public virtual int TenantId { get; set; }

        public abstract long UserId { get; set; }

        [StringLength(MaxClaimTypeLength)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }
        public FrontUserClaim() { }
        public FrontUserClaim(int tenantId, long userId, Claim claim)
        {
            UserId = userId;
            TenantId = tenantId;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
