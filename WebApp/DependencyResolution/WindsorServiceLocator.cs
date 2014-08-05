using FubuCore;
using Castle.MicroKernel;

namespace WebApp.DependencyResolution
{
    public class WindsorServiceLocator : IServiceLocator
    {
        private readonly IKernel _container;

        public WindsorServiceLocator(IKernel container)
        {
            _container = container;
        }

        public T GetInstance<T>(string name)
        {
            return _container.Resolve<T>(name);
        }

        public object GetInstance(System.Type type)
        {
            return _container.Resolve(type);
        }

        public T GetInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}