using JK.Payments.Enumerates;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// 响应值
    /// </summary>
    public class ResponseValueConfiguration
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        [Required]
        public string ResponseValue { get; set; }

        public ResponseValueMean Mean { get; set; }

    }
}
