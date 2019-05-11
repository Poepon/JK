using System;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.Bacis
{
    /// <summary>
    /// 银行
    /// </summary>
    public class Bank
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string BankCode { get; set; }

        public int OrderNumber { get; set; }

    }
}
