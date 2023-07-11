namespace Fab.Entities.Abstractions.Interfaces;

public interface ISoftDeletes
{
    DateTime? DeletedAt { get; set; }
}