using FubuCore;
using StructureMap;

namespace WebApp.DependencyResolution
{
    public class StructureMapServiceLocator : IServiceLocator
    {
        private readonly IContainer _container;

        public StructureMapServiceLocator(IContainer container)
        {
            _container = container;
        }

        public T GetInstance<T>(string name)
        {
            return _container.GetInstance<T>(name);
        }

        public object GetInstance(System.Type type)
        {
            return _container.GetInstance(type);
        }

        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}