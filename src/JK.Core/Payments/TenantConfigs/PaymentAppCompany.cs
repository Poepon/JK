using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Integration;

namespace JK.Payments.TenantConfigs
{
    [Table("PaymentAppCompany")]
    public class PaymentAppCompany
    {
        public int AppId { get; set; }

        [ForeignKey(nameof(AppId))]
        public virtual PaymentApp App { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
    }
}
