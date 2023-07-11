using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Entities.Specifications;
using Fab.UseCases.Dto;
using Fab.Utils.Extensions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using static Fab.Entities.Models.Communications.Communication;

namespace Fab.UseCases.Validators;

public static class ValidationExtensions
{
    #region Extensions for external validators

    public static IRuleBuilderOptions<T, string> Communication<T>(
        this IRuleBuilder<T, string> ruleBuilder, Func<T, CommunicationType> communicationTypeResolver) =>
        ruleBuilder.SetValidator(new CommunicationValidator<T>(communicationTypeResolver));

    public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new PhoneValidator<T>());

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new EmailValidator<T>());

    public static IRuleBuilderOptions<T, string> Tin<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new TinValidator<T>());

    public static IRuleBuilderOptions<T, string> Bik<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new BikValidator<T>());

    public static IRuleBuilderOptions<T, string> Ks<T>(
        this IRuleBuilder<T, string> ruleBuilder, Func<T, string> bikResolver) =>
        ruleBuilder.SetValidator(new KsValidator<T>(bikResolver));

    public static IRuleBuilderOptions<T, string> Rs<T>(
        this IRuleBuilder<T, string> ruleBuilder, Func<T, string> bikResolver) =>
        ruleBuilder.SetValidator(new RsValidator<T>(bikResolver));

    public static IRuleBuilderOptions<T, string> Kpp<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new KppValidator<T>());

    public static IRuleBuilderOptions<T, string> Ogrn<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new OgrnValidator<T>());

    public static IRuleBuilderOptions<T, string> Ogrnip<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new OgrnipValidator<T>());

    public static IRuleBuilderOptions<T, string> Okved<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.SetValidator(new OkvedValidator<T>());

    #endregion

    #region Simple Rules

    public static IRuleBuilderOptions<T, ICollection<TElement>> Unique<T, TElement>(
        this IRuleBuilder<T, ICollection<TElement>> ruleBuilder, IEqualityComparer<TElement>? comparer = default) =>
        ruleBuilder.Must(x => x.Distinct(comparer).Count() == x.Count);

    public static IRuleBuilderOptions<T, string> OnlyDigits<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Must(x => x.All(char.IsDigit));

    #endregion

    #region ForEntity

    public static IRuleBuilderOptions<T, Guid> ForEntity<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, IQueryable<TEntity> query,
        Action<InlineValidator<TEntity>> configureValidator)
        where TEntity : class, IEntity =>
        ruleBuilder.ForEntity(_ => query, (_, v) => configureValidator(v));

    public static IRuleBuilderOptions<T, Guid> ForEntity<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, IQueryable<TEntity> query,
        Action<T, InlineValidator<TEntity>> configureValidator)
        where TEntity : class, IEntity =>
        ruleBuilder.ForEntity(_ => query, configureValidator);

    public static IRuleBuilderOptions<T, Guid> ForEntity<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver,
        Action<InlineValidator<TEntity>> configureValidator)
        where TEntity : class, IEntity =>
        ruleBuilder.ForEntity(queryResolver, (_, v) => configureValidator(v));

    public static IRuleBuilderOptions<T, Guid> ForEntity<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver,
        Action<T, InlineValidator<TEntity>> configureValidator)
        where TEntity : class, IEntity =>
        ruleBuilder.MustAsync(async (request, id, ctx, cancellationToken) =>
                   {
                       var entity = await queryResolver(request).AsNoTracking()
                                                                .ById(id)
                                                                .FirstOrDefaultAsync(cancellationToken);

                       if (entity == null)
                       {
                           return false;
                       }

                       var r = await new InlineValidator<TEntity>()
                                     .Also(x => configureValidator(request, x))
                                     .ValidateAsync(entity, cancellationToken);

                       foreach (var error in r.Errors)
                       {
                           error.PropertyName = new PropertyChain()
                                                .Also(x => x.Add(ctx.PropertyName))
                                                .Also(x => x.Add(error.PropertyName))
                                                .ToString();

                           ctx.AddFailure(error);
                       }

                       return true;
                   })
                   .WithMessage("Сущность не найдена")
                   .WithErrorCode("EntityNotFound");

    #endregion

    #region Exists

    public static IRuleBuilderOptions<T, Guid> Exists<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, IQueryable<TEntity> query)
        where TEntity : class, IEntity =>
        ruleBuilder.Exists(_ => query);

    public static IRuleBuilderOptions<T, Guid> Exists<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver)
        where TEntity : class, IEntity =>
        ruleBuilder.MustAsync((request, id, cancellationToken) =>
                       queryResolver(request).AsNoTracking()
                                             .ById(id)
                                             .AnyAsync(cancellationToken))
                   .WithMessage("Сущность не найдена")
                   .WithErrorCode("EntityNotFound");

    public static IRuleBuilderOptionsConditions<T, ICollection<Guid>> Exists<T, TEntity>(
        this IRuleBuilder<T, ICollection<Guid>> ruleBuilder, IQueryable<TEntity> query)
        where TEntity : class, IEntity =>
        ruleBuilder.Exists(_ => query);

    public static IRuleBuilderOptionsConditions<T, ICollection<Guid>> Exists<T, TEntity>(
        this IRuleBuilder<T, ICollection<Guid>> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver,
        Func<string, ValidationFailure>? messageFactory = null)
        where TEntity : class, IEntity =>
        ruleBuilder.CustomAsync(async (ids, ctx, cancellationToken) =>
        {
            var entities = await queryResolver(ctx.InstanceToValidate).AsNoTracking()
                                                                      .Where(x => ids.Contains(x.Id))
                                                                      .Select(x => x.Id)
                                                                      .ToListAsync(cancellationToken);

            static ValidationFailure DefaultMessageFactory(string propertyName) =>
                new(propertyName, "Сущность не найдена")
                {
                    ErrorCode = "EntityNotFound"
                };

            foreach (var collision in ids.Select((x, i) => new { Index = i, Id = x })
                                         .Where(x => !entities.Contains(x.Id)))
            {
                var propertyName = new PropertyChain()
                                   .Also(x => x.Add(ctx.PropertyName))
                                   .Also(x => x.AddIndexer(collision.Index))
                                   .ToString();

                ctx.AddFailure((messageFactory ?? DefaultMessageFactory)(propertyName));
            }
        });

    #endregion

    #region NotExists

    public static IRuleBuilderOptions<T, Guid> NotExists<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, IQueryable<TEntity> query)
        where TEntity : class, IEntity =>
        ruleBuilder.NotExists(_ => query);

    public static IRuleBuilderOptions<T, Guid> NotExists<T, TEntity>(
        this IRuleBuilder<T, Guid> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver)
        where TEntity : class, IEntity =>
        ruleBuilder.MustAsync((request, id, cancellationToken) =>
                       queryResolver(request).AsNoTracking()
                                             .ById(id)
                                             .AnyAsync(cancellationToken)
                                             .ContinueWith(x => !x.Result, cancellationToken))
                   .WithMessage("Сущность уже существует")
                   .WithErrorCode("EntityAlreadyExists");

    public static IRuleBuilderOptionsConditions<T, ICollection<Guid>> NotExists<T, TEntity>(
        this IRuleBuilder<T, ICollection<Guid>> ruleBuilder, IQueryable<TEntity> query)
        where TEntity : class, IEntity =>
        ruleBuilder.NotExists(_ => query);

    public static IRuleBuilderOptionsConditions<T, ICollection<Guid>> NotExists<T, TEntity>(
        this IRuleBuilder<T, ICollection<Guid>> ruleBuilder, Func<T, IQueryable<TEntity>> queryResolver,
        Func<string, ValidationFailure>? messageFactory = null)
        where TEntity : class, IEntity =>
        ruleBuilder.CustomAsync(async (ids, ctx, cancellationToken) =>
        {
            var entities = await queryResolver(ctx.InstanceToValidate).AsNoTracking()
                                                                      .Where(x => ids.Contains(x.Id))
                                                                      .Select(x => x.Id)
                                                                      .ToListAsync(cancellationToken);

            static ValidationFailure DefaultMessageFactory(string propertyName) =>
                new(propertyName, "Сущность уже существует")
                {
                    ErrorCode = "EntityAlreadyExists"
                };

            foreach (var collision in ids.Select((x, i) => new { Index = i, Id = x })
                                         .Where(x => entities.Contains(x.Id)))
            {
                var propertyName = new PropertyChain()
                                   .Also(x => x.Add(ctx.PropertyName))
                                   .Also(x => x.AddIndexer(collision.Index))
                                   .ToString();

                ctx.AddFailure((messageFactory ?? DefaultMessageFactory)(propertyName));
            }
        });

    #endregion

    #region UniqueCommunications

    public static IRuleBuilderOptionsConditions<T, ICollection<CommunicationShortDto>> UniqueCommunications<T>(
        this IRuleBuilder<T, ICollection<CommunicationShortDto>> ruleBuilder,
        IQueryable<Communication> query, Func<string, ValidationFailure>? messageFactory = null) =>
        ruleBuilder.UniqueCommunications(_ => query, messageFactory);

    public static IRuleBuilderOptionsConditions<T, ICollection<CommunicationShortDto>> UniqueCommunications<T>(
        this IRuleBuilder<T, ICollection<CommunicationShortDto>> ruleBuilder,
        Func<T, IQueryable<Communication>> queryResolver,
        Func<string, ValidationFailure>? messageFactory = null) =>
        ruleBuilder.CustomAsync(async (request, ctx, cancellationToken) =>
        {
            if (request is not { Count: > 0 })
            {
                return;
            }

            var existed = await queryResolver(ctx.InstanceToValidate)
                                .AsNoTracking()
                                .Where(request.Select(x => ByTypeAndValue(x.Type, x.Value))
                                              .Aggregate((acc, val) => acc | val))
                                .Select(x => new CommunicationShort(x.Type, x.Value))
                                .ToListAsync(cancellationToken);

            if (existed.Count == 0)
            {
                return;
            }

            static ValidationFailure DefaultMessageFactory(string propertyName) =>
                new(propertyName, "Коммуникация уже занята")
                {
                    ErrorCode = "CommunicationAlreadyBusy"
                };

            foreach (var collision in request.Select((x, i) => new
                                             {
                                                 Index = i,
                                                 Communication = x
                                             })
                                             .Where(x => existed.Any(c =>
                                                 c.Type == x.Communication.Type &&
                                                 c.Value == x.Communication.Value)))
            {
                var propertyName = new PropertyChain()
                                   .Also(x => x.Add(ctx.PropertyName))
                                   .Also(x => x.AddIndexer(collision.Index))
                                   .ToString();

                ctx.AddFailure((messageFactory ?? DefaultMessageFactory)(propertyName));
            }
        });

    #endregion
}