using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JK.Front;

namespace JK.Customers
{
    /// <summary>
    /// Used to save a login attempt of a customer.
    /// </summary>
    [Table("CustomerLoginAttempts")]
    public class CustomerLoginAttempt : FrontUserLoginAttempt
    {
        [Column("CustomerId")]
        public override long? UserId { get; set; }
    }
}
