using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace WebApp.DependencyResolution
{
    public class WindsorDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _container;

        public WindsorDependencyResolver(IKernel container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return serviceType.IsAssignableFrom(typeof(IKernel))
                ? _container
                : _container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }
    }
}