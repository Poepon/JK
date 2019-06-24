using JK.Payments.Bacis;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Payments
{
    public static class StaticContext
    {
        public static List<Bank> Banks { get; set; }

        public static List<Channel> Channels { get; set; }
    }
}
