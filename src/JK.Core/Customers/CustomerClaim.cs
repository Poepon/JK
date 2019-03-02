using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace JK.Customers
{
    [Table("CustomerClaims")]
    public class CustomerClaim : FrontUserClaim
    {
        public CustomerClaim() { }
        public CustomerClaim(int tenantId, long userId, Claim claim) : base(tenantId, userId, claim)
        {
        }

        [Column("CustomerId")]
        public override long UserId { get; set; }

      
    }
}
