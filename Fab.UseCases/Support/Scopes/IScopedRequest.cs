using Fab.Entities.Abstractions;
using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Support.Scopes;

public interface IScopedRequest<TEntity>
    where TEntity : IEntity
{
    public Spec<TEntity>? Scope { get; set; }
}