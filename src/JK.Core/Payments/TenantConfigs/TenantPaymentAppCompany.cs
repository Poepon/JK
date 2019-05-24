using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Integration;

namespace JK.Payments.TenantConfigs
{
    [Table("TenantPaymentAppCompany")]
    public class TenantPaymentAppCompany
    {
        public int AppId { get; set; }

        [ForeignKey(nameof(AppId))]
        public virtual TenantPaymentApp App { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
    }
}
