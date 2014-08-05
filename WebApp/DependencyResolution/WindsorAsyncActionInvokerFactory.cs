using Castle.MicroKernel;
using System.Web.Mvc.Async;

namespace WebApp.DependencyResolution
{
    public class WindsorAsyncActionInvokerFactory : IAsyncActionInvokerFactory
    {
        private readonly IKernel _kernel;

        public WindsorAsyncActionInvokerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IAsyncActionInvoker CreateInstance()
        {
            return _kernel.Resolve<IAsyncActionInvoker>();
        }
    }
}
