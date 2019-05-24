using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace JK.Front
{
    public abstract class FrontUserLogin : Entity<long>
    {
        public FrontUserLogin() { }
        public FrontUserLogin(int tenantId, long userId, string loginProvider, string providerKey)
        {
            TenantId = tenantId;
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
        /// <summary>
        /// Maximum length of <see cref="LoginProvider"/> property.
        /// </summary>
        public const int MaxLoginProviderLength = 128;

        /// <summary>
        /// Maximum length of <see cref="ProviderKey"/> property.
        /// </summary>
        public const int MaxProviderKeyLength = 256;

        public virtual int TenantId { get; set; }

        /// <summary>
        /// Id of the User.
        /// </summary>
        public abstract long UserId { get; set; }

        /// <summary>
        /// Login Provider.
        /// </summary>
        [Required]
        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Key in the <see cref="LoginProvider"/>.
        /// </summary>
        [Required]
        [StringLength(MaxProviderKeyLength)]
        public virtual string ProviderKey { get; set; }

    }
}
