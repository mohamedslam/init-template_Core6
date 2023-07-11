using AltPoint.Filters.Definitions;
using Autofac;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Fab.UseCases.Behaviors;
using Fab.UseCases.Validators;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Module = Autofac.Module;

#pragma warning disable CS0618

namespace Fab.UseCases;

public class UseCasesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var mediatrOpenTypes = new[]
        {
            // MediatR
            typeof(IRequestHandler<,>),
            typeof(IRequestExceptionHandler<,,>),
            typeof(IRequestExceptionAction<,>),
            typeof(INotificationHandler<>),

            // FluentValidatiom
            typeof(IValidator<>),
        };

        foreach (var mediatrOpenType in mediatrOpenTypes)
        {
            builder
                .RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
                .AsClosedTypesOf(mediatrOpenType)
                // when having a single class implementing several handler types
                // this call will cause a handler to be called twice
                // in general you should try to avoid having a class implementing for instance `IRequestHandler<,>` and `INotificationHandler<>`
                // the other option would be to remove this call
                // see also https://github.com/jbogard/MediatR/issues/462
                .AsImplementedInterfaces();
        }

        // It appears Autofac returns the last registered types first
        builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(RequestExceptionActionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        builder.Register<ServiceFactory>(ctx => ctx.Resolve<IComponentContext>()
                                                   .Resolve);

        builder.RegisterGeneric(typeof(PaginationValidator<>)).As(typeof(IValidator<>));

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .AssignableTo<Profile>()
               .PublicOnly()
               .As<Profile>();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .AssignableTo<IFilterDefinition>()
               .PublicOnly()
               .AsImplementedInterfaces();

        builder.Register(ctx =>
               {
                   var config = new MapperConfiguration(cfg =>
                   {
                       cfg.AllowNullCollections = false;
                       cfg.SourceMemberNamingConvention = new ExactMatchNamingConvention();
                       cfg.DestinationMemberNamingConvention = new ExactMatchNamingConvention();

                       cfg.AddCollectionMappers();
                       cfg.DisableConstructorMapping();
                       cfg.AddProfiles(ctx.Resolve<IEnumerable<Profile>>());
                   });

                   config.AssertConfigurationIsValid();
                   config.CompileMappings();

                   return config;
               })
               .AsSelf()
               .AutoActivate()
               .SingleInstance();

        builder.Register(ctx => ctx.Resolve<MapperConfiguration>()
                                   .CreateMapper(ctx.Resolve<IComponentContext>()
                                                    .Resolve))
               .InstancePerLifetimeScope();

        builder.RegisterBuildCallback(ConfigureValidator);
    }

    private static void ConfigureValidator(ILifetimeScope scope)
    {
        var namingPolicy = scope.Resolve<IOptions<JsonSerializerOptions>>()
                                .Value.PropertyNamingPolicy;

        if (namingPolicy == null)
        {
            return;
        }

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        ValidatorOptions.Global.PropertyNameResolver = (_, member, lambda) =>
        {
            if (lambda?.Body is MemberExpression me)
            {
                return namingPolicy.ConvertName(me.Member.Name);
            }

            return member != null
                ? namingPolicy.ConvertName(member.Name)
                : null;
        };
    }
}