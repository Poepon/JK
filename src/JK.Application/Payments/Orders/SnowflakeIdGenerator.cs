using System;

namespace JK.Payments.Orders
{
    public class SnowflakeIdGenerator: IIdGenerator
    {
        /// <summary>
        /// 数据中心编号。取值为0-31
        /// </summary>
        private uint dataCenterId = 0;
        /// <summary>
        /// 机器编号。取值为0-31
        /// </summary>
        private uint workerId = 0;
        //最后的时间戳
        private long lastTimestamp = -1L;
        //最后的序号
        private int lastIndex = -1;
      
        static object locker = new object();
       
        public void Init(uint dataCenterId, uint workerId)
        {
            if (dataCenterId > 31)
            {
                throw new Exception("数据中心取值范围为0-31");
            }
            if (workerId > 31)
            {
                throw new Exception("机器码取值范围为0-31");
            }
            this.dataCenterId = dataCenterId;
            this.workerId = workerId;
        }

        public long NextId()
        {
            lock (locker)
            {
                var currentTimeStamp = GetNowTimeStamp();
                if (currentTimeStamp < lastTimestamp)
                {
                    throw new Exception("时间戳生成出现错误");
                }
                if (currentTimeStamp == lastTimestamp)
                {
                    if (lastIndex < 4095)//为了保证长度
                    {
                        lastIndex++;
                    }
                    else
                    {
                        currentTimeStamp = WaitNextMillisecond(lastTimestamp);
                        lastIndex = 0;
                        lastTimestamp = currentTimeStamp;
                    }
                }
                else
                {
                    lastIndex = 0;
                    lastTimestamp = currentTimeStamp;
                }
                var timeStr = Convert.ToString(currentTimeStamp, 2);
                var dcStr = Convert.ToString(dataCenterId, 2).PadLeft(5, '0');
                var wStr = Convert.ToString(workerId, 2).PadLeft(5, '0'); ;
                var indexStr = Convert.ToString(lastIndex, 2).PadLeft(12, '0');
                return Convert.ToInt64($"0{timeStr}{dcStr}{wStr}{indexStr}", 2);
            }

        }

        public long WaitNextMillisecond(long lastTimestamp)
        {
            long timestamp = GetNowTimeStamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetNowTimeStamp();
            }
            return timestamp;
        }

        public long GetNowTimeStamp()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
