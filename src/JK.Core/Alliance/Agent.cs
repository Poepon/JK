using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JK.Alliance
{
    [Table("Agents")]
    public class Agent : FullAuditedEntity<long>
    {

    }
}
