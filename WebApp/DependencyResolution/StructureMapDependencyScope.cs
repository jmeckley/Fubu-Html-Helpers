using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace WebApp.DependencyResolution
{

    public class StructureMapDependencyScope : ServiceLocatorImplBase
    {
        private const string NestedContainerKey = "Nested.Container.Key";

        public StructureMapDependencyScope(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            Container = container;
        }

        public IContainer Container { get; set; }

        public IContainer CurrentNestedContainer
        {
            get { return (IContainer) HttpContext.Items[NestedContainerKey]; }
            set { HttpContext.Items[NestedContainerKey] = value; }
        }

        private HttpContextBase HttpContext
        {
            get
            {
                var ctx = Container.TryGetInstance<HttpContextBase>();
                return ctx ?? new HttpContextWrapper(System.Web.HttpContext.Current);
            }
        }

        public void CreateNestedContainer()
        {
            if (CurrentNestedContainer != null) return;
            CurrentNestedContainer = Container.GetNestedContainer();
        }

        public void Dispose()
        {
            CurrentNestedContainer?.Dispose();

            Container.Dispose();
        }

        public void DisposeNestedContainer()
        {
            CurrentNestedContainer?.Dispose();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return DoGetAllInstances(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (CurrentNestedContainer ?? Container).GetAllInstances(serviceType).Cast<object>();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            var container = CurrentNestedContainer ?? Container;

            if (string.IsNullOrEmpty(key))
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                    ? container.TryGetInstance(serviceType)
                    : container.GetInstance(serviceType);
            }

            return container.GetInstance(serviceType, key);
        }
    }
}
