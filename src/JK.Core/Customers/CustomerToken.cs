using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JK.Front;

namespace JK.Customers
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    [Table("CustomerTokens")]
    public class CustomerToken : FrontUserToken
    {
        public CustomerToken() { }
        public CustomerToken(int tenantId, long userId, [NotNull] string loginProvider, [NotNull] string name, string value, DateTime? expireDate = null) : base(tenantId, userId, loginProvider, name, value, expireDate)
        {
        }

        [Column("CustomerId")]
        public override long UserId { get; set; }
    }
}
