using Castle.Facilities.Logging;

namespace Abp.Castle.Exceptionless
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseAbpExceptionless(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<ExceptionlessLoggerFactory>();
        }
    }
}
