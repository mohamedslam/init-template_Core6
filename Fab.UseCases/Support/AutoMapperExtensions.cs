using AutoMapper;
using Fab.Entities.Abstractions.Interfaces;
using Fab.UseCases.Support.Scopes;
using Fab.Utils.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace Fab.UseCases.Support;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination>
        Map<TSource, TSourceMember, TDestination, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> config,
            Expression<Func<TDestination, TDestinationMember>> destination,
            Expression<Func<TSource, TSourceMember>> source) =>
        config.ForMember(destination, opt => opt.MapFrom(source));

    public static IProjectionExpression<TSource, TDestination>
        Map<TSource, TSourceMember, TDestination, TDestinationMember>(
            this IProjectionExpression<TSource, TDestination> config,
            Expression<Func<TDestination, TDestinationMember>> destination,
            Expression<Func<TSource, TSourceMember>> source) =>
        config.ForMember(destination, opt => opt.MapFrom(source));

    public static IMappingExpression<TSource, TDestination>
        Map<TSource, TSourceMember, TDestination, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> config,
            Expression<Func<TDestination, TDestinationMember>> destination,
            Expression<Func<TSource, TSourceMember>> source,
            Func<TSource, bool> shouldMap) =>
        config.ForMember(destination, opt =>
        {
            opt.PreCondition(shouldMap);
            opt.MapFrom(source);
        });

    public static IMappingExpression<TSource, TDestination>
        Map<TSource, TDestination, TDestinationMember, TResult>(
            this IMappingExpression<TSource, TDestination> config,
            Expression<Func<TDestination, TDestinationMember>> destination,
            Func<TSource, TDestination, TResult> source) =>
        config.ForMember(destination, opt => opt.MapFrom(source));

    public static IMappingExpression<TSource, TDestination>
        Map<TSource, TDestination, TDestinationMember, TResult>(
            this IMappingExpression<TSource, TDestination> config,
            Expression<Func<TDestination, TDestinationMember>> destination,
            Func<TSource, TDestination, ResolutionContext, TResult> source) =>
        config.ForMember(destination, opt => opt.MapFrom((src, dest, _, ctx) => source(src, dest, ctx)));

    public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> config,
        Expression<Func<TDestination, TDestinationMember>> destination) =>
        config.ForMember(destination, opt => opt.Ignore());

    public static IProjectionExpression<TSource, TDestination> Ignore<TSource, TDestination, TDestinationMember>(
        this IProjectionExpression<TSource, TDestination> config,
        Expression<Func<TDestination, TDestinationMember>> destination) =>
        config.ForMember(destination, opt => opt.Ignore());

    public static IMappingExpression<TSource, TDestination> IgnoreDefaults<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> config)
    {
        if (typeof(TSource).GetInterfaces()
                           .Any(t => t.IsGenericType &&
                                     t.GetGenericTypeDefinition() == typeof(IScopedRequest<>)))
        {
            config.ForSourceMember(nameof(IScopedRequest<IEntity>.Scope), opt => opt.DoNotValidate());
        }

        if (typeof(TDestination).IsAssignableTo(typeof(IEntity)))
        {
            config.ForMember(nameof(IEntity.Id), opt => opt.Ignore());
        }

        if (typeof(TDestination).GetProperty("Discriminator",
                BindingFlags.Instance | BindingFlags.Public) != null)
        {
            config.ForMember("Discriminator", opt => opt.Ignore());
        }

        if (typeof(TDestination).IsAssignableTo(typeof(IHasTimestamps)))
        {
            config.ForMember(nameof(IHasTimestamps.CreatedAt), opt => opt.Ignore())
                  .ForMember(nameof(IHasTimestamps.UpdatedAt), opt => opt.Ignore());
        }

        if (typeof(TDestination).IsAssignableTo(typeof(ISoftDeletes)))
        {
            config.ForMember(nameof(ISoftDeletes.DeletedAt), opt => opt.Ignore());
        }

        return config;
    }

    public static T GetService<T>(this ResolutionContext context) =>
        context.Options.ServiceCtor(typeof(T)).As<T>();
}