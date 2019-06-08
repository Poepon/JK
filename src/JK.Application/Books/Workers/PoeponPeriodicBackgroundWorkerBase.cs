using System;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;

namespace JK.Books.Workers
{
    public abstract class PoeponPeriodicBackgroundWorkerBase : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        protected Guid WorkerId = Guid.NewGuid();
        protected bool WorkerIsRunning = false;
        protected PoeponPeriodicBackgroundWorkerBase(AbpTimer timer) : base(timer)
        {
        }
        [UnitOfWork(IsDisabled = false)]
        protected override void DoWork()
        {

            if (WorkerIsRunning == false)
            {
                try
                {
                    WorkerIsRunning = true;
                    Logger.Info($"{this.GetType().Name}[{WorkerId.ToString()}] Start.");
                    DoSomething();
                    Logger.Info($"{this.GetType().Name}[{WorkerId.ToString()}] End.");
                }
                catch (Exception e)
                {
                    Logger.Error($"{this.GetType().Name}[{WorkerId.ToString()}] Error.",e);
                }
                finally
                {
                    WorkerIsRunning = false;
                }
            }
            else
            {
                Logger.Warn($"{this.GetType().Name}[{WorkerId.ToString()}] is running.");
            }
        }

        protected abstract void DoSomething();

    }
}
