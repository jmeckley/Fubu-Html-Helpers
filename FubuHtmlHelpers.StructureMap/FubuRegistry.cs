using FubuCore;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Security;
using HtmlTags.Conventions;
using StructureMap;

namespace FubuHtmlHelpers.StructureMap
{
    public class FubuRegistry 
        : Registry
    {
        public FubuRegistry()
        {
            Scan(scan =>
            {
                scan.AssemblyContainingType<IFubuRequest>();
                scan.AssemblyContainingType<ITypeResolver>();
                scan.AssemblyContainingType<ITagGeneratorFactory>();
                scan.AssemblyContainingType<IFieldAccessService>();

                scan.WithDefaultConventions();
                scan.LookForRegistries();
            });

            //seems like this should take care of itself, but it's not...
            For<IServiceLocator>().Use<StructureMapServiceLocator>();
            For<IBindingLogger>().Use<NulloBindingLogger>();
        }
    }
}