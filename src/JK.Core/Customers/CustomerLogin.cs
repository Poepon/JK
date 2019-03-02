using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Customers
{
    /// <summary>
    /// Used to store a Customer Login for external Login services.
    /// </summary>
    [Table("CustomerLogins")]
    public class CustomerLogin : FrontUserLogin
    {
        public CustomerLogin() { }
        public CustomerLogin(int tenantId, long userId, string loginProvider, string providerKey) : base(tenantId, userId, loginProvider, providerKey)
        {
        }

        [Column("CustomerId")]
        public override long UserId { get; set; }

       
    }
}
