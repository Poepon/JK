using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;
using JK.Front;

namespace JK.Alliance
{
    /// <summary>
    /// Used to store a Customer Login for external Login services.
    /// </summary>
    [Table("AgentLogins")]
    public class AgentLogin : FrontUserLogin
    {
        public AgentLogin() { }
        public AgentLogin(int tenantId, long userId, string loginProvider, string providerKey) : base(tenantId, userId, loginProvider, providerKey)
        {
        }

        [Column("AgentId")]
        public override long UserId { get; set; }

       
    }
}
