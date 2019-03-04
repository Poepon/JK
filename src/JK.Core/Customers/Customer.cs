using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Customers
{
    [Table("Customers")]
    public class Customer : FrontUserBase<CustomerLogin, CustomerClaim, CustomerToken>
    {

        [ForeignKey("CustomerId")]
        public override ICollection<CustomerLogin> Logins { get; set; }

        [ForeignKey("CustomerId")]
        public override ICollection<CustomerClaim> Claims { get; set; }

        [ForeignKey("CustomerId")]
        public override ICollection<CustomerToken> Tokens { get; set; }

        public long? AgentId { get; set; }

        public long? RefCustomerId { get; set; }
    }
}
