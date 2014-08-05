using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApp.DependencyResolution
{
    public class WindsorViewPageActivator : IViewPageActivator
    {
        public const string key = "__WINDSOR_VIEWS__";

        private readonly IKernel _kernel;

        public WindsorViewPageActivator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Create(ControllerContext controllerContext, Type type)
        {
            var view = ResolveView(type);

            Cache(view);

            return view;
        }

        private object ResolveView(Type type)
        {
            if (_kernel.HasComponent(type.FullName) == false)
            {
                _kernel.Register(Component.For(type).Named(type.FullName).Activator<ViewPageComponentActivator>().LifestyleTransient());
            }

            return _kernel.Resolve(type.FullName, type);
        }

        private void Cache(object view)
        {
            var context = _kernel.Resolve<HttpContextBase>();
            if(context.Items.Contains(key) == false)
            {
                context.Items.Add(key, new List<object>());
            }

            ((List<object>)context.Items[key]).Add(view);
        }
    }

    public class ViewPageComponentActivator : DefaultComponentActivator
    {
        public ViewPageComponentActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            // Do like the MVC framework.
            return Activator.CreateInstance(context.RequestedType);
        }
    }
}
