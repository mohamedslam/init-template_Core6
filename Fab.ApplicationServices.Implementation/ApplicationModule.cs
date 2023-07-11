using Autofac;
using System.Reflection;
using Module = Autofac.Module;

namespace Fab.ApplicationServices.Implementation;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
            .PublicOnly()
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}