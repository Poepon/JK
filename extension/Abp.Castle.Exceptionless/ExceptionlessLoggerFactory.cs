using Castle.Core.Logging;
using System;
namespace Abp.Castle.Exceptionless
{
    public class ExceptionlessLoggerFactory : AbstractLoggerFactory
    {
        public override ILogger Create(string name)
        {
            throw new NotImplementedException();
        }

        public override ILogger Create(string name, LoggerLevel level)
        {
            throw new NotImplementedException();
        }
    }
    
}
