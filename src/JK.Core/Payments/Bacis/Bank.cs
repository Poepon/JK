using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.Bacis
{
    /// <summary>
    /// 银行
    /// </summary>
    [Table("Banks")]
    public class Bank : Entity
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
