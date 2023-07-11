namespace Fab.UseCases.Support.Pagination;

internal sealed class NormalizedPaginationRequest : IPaginationRequest
{
    private readonly IPaginationRequest _request;
    private readonly PageOptions _options;

    public int Page => Math.Max(_request.Page, 1);

    public int? Limit => _request.Limit.HasValue
        ? Math.Max(_options.MaxPageItems.HasValue
            ? Math.Min(_options.MaxPageItems.Value, _request.Limit.Value)
            : _request.Limit.Value, 0)
        : null;

    public NormalizedPaginationRequest(IPaginationRequest request, PageOptions options)
    {
        _request = request;
        _options = options;
    }
}