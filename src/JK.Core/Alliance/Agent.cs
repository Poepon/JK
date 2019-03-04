using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Alliance
{
    [Table("Agents")]
    public class Agent : FrontUserBase<AgentLogin, AgentClaim, AgentToken>
    {
        [ForeignKey("AgentId")]
        public override ICollection<AgentLogin> Logins { get; set; }

        [ForeignKey("AgentId")]
        public override ICollection<AgentClaim> Claims { get; set; }

        [ForeignKey("AgentId")]
        public override ICollection<AgentToken> Tokens { get; set; }

        public long? ParentId { get; set; }
    }
}
