using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace JK
{
    public interface IAppContext
    {
        string LocalHostName { get; set; }
    }
    public class AppContext : IAppContext, ISingletonDependency
    {
        public string LocalHostName { get; set; }
    }

}
