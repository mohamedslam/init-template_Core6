namespace Fab.Entities.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class SearchableAttribute : Attribute
{
    public SearchMode Mode { get; set; } = SearchMode.Contains;
}