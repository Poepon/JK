using System.ComponentModel.DataAnnotations.Schema;
using JK.Front;

namespace JK.Alliance
{
    [Table("AgentLoginAttempts")]
    public class AgentLoginAttempt : FrontUserLoginAttempt
    {
        [Column("AgentId")]
        public override long? UserId { get; set; }
    }
}
