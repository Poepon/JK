using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Payments.Enumerates
{
    /// <summary>
    /// 交互条件
    /// </summary>
    public enum RuleGroupInteractionType
    {
        /// <summary>
        /// 必须满足集合内的所有要求
        /// </summary>
        And = 0,

        /// <summary>
        /// 必须满足集合内至少一个要求
        /// </summary>
        Or = 2,
    }
}
