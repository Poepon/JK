using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Alliance
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    [Table("AgentTokens")]
    public class AgentToken : FrontUserToken
    {
        public AgentToken() { }
        public AgentToken(int tenantId, long userId, [NotNull] string loginProvider, [NotNull] string name, string value, DateTime? expireDate = null) : base(tenantId, userId, loginProvider, name, value, expireDate)
        {
        }

        [Column("AgentId")]
        public override long UserId { get; set; }
    }
}
