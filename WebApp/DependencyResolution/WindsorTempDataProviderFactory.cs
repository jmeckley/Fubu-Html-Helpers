using Castle.MicroKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace WebApp.DependencyResolution
{
    public class WindsorTempDataProviderFactory : ITempDataProviderFactory
    {
        private readonly IKernel _kernel;

        public WindsorTempDataProviderFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ITempDataProvider CreateInstance()
        {
            return _kernel.Resolve<ITempDataProvider>();
        }
    }
}
