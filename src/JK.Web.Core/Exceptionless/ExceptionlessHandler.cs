using Abp.Authorization;
using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Exceptionless;
using System.Threading.Tasks;

namespace JK.Exceptionless
{
    public class ExceptionlessHandler : IAsyncEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public Task HandleEventAsync(AbpHandledExceptionData eventData)
        {
            if (!(eventData.Exception is AbpAuthorizationException))
            {
                eventData.Exception.ToExceptionless()
                    .MarkAsCritical()
                    .AddObject(eventData.EventSource)
                    .Submit();
            }
                
            return Task.CompletedTask;
        }
    }

}
