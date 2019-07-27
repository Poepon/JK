using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JK.Payments.Orders.Dto
{
    public class ParameterValueResult : List<ParameterValue>
    {
        public string this[string key]
        {
            get
            {
                var item = this.FirstOrDefault(v => v.Key == key);
                return item?.Value;
            }
            set
            {
                if (this.Any(v => v.Key == key))
                {
                    this.First(v => v.Key == key).Value = value;
                }
                else
                {
                    throw new ArgumentException($"Key:{key}不存在", nameof(key));
                }
            }
        }
    }

}
