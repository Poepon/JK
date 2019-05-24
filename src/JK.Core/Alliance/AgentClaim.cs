using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using JK.Front;

namespace JK.Alliance
{
    [Table("AgentClaims")]
    public class AgentClaim : FrontUserClaim
    {
        public AgentClaim() { }
        public AgentClaim(int tenantId, long userId, Claim claim) : base(tenantId, userId, claim)
        {
        }

        [Column("AgentId")]
        public override long UserId { get; set; }

      
    }
}
